'use client';

import { usePathname, useRouter } from 'next/navigation';
import { getRoleLayout } from './config/roles';
import { useEffect } from 'react';
import { hrmsAPI } from './lib/api/client';
import MainBody from './Components/mainBody';
import Header from './Components/Header';

export default function ClientWrapper({ children, session }) {
  const router = useRouter();
  const pathname = usePathname() || '/';

  const role = session?.user?.role;
  const requiresOtp = session?.requiresOtp;
  const otpVerified = session?.otpVerified;

  const defaultPaths = {
    SuperAdmin: 'SuperAdmin/AllOrganization',
    HR: 'Admin/Dashboard',
    Employee: 'EmployeePortal/Dashboard',
    SystemAdmin: 'Admin/RecruitmentPages/Jobposting',
  };

  // ✅ OTP enforcement
  useEffect(() => {
    if (
      requiresOtp &&
      !otpVerified &&
      !pathname.startsWith('/Login/VerifyOtp')
    ) {
      router.push(
        `/Login/VerifyOtp?username=${session?.user?.id || session?.user?.email}`
      );
    }
  }, [requiresOtp, otpVerified, pathname, router, session]);

  // ✅ Update last login
  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token || !session?.user?.id || (requiresOtp && !otpVerified)) return;

    const updateLastLogin = async () => {
      try {
        await hrmsAPI.touchLogin(session.user.id);
      } catch (err) {
        console.error('Failed to update last login:', err);
      }
    };

    updateLastLogin();
  }, [session?.user?.id, requiresOtp, otpVerified]);

  // ✅ Path & layout resolution
  const uuidRegex =
    /\/[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}/g;

  const cleanPath =
    pathname === '/' ? defaultPaths[role] : pathname.replace('/', '');

  const readPath = cleanPath.replace(uuidRegex, '');

  const {
    body: BodyComponent,
    header: HeaderComponent,
  } = getRoleLayout(role, readPath);

  // ✅ Unauthorized page stays untouched
  if (pathname.startsWith('/Unauthorized')) {
    return <div className="bg-black">{children}</div>;
  }

  // ✅ OTP-only rendering
  if (requiresOtp && !otpVerified) {
    if (pathname.startsWith('/Login/VerifyOtp')) {
      return <div>{children}</div>;
    }
    return <div>Redirecting to OTP verification...</div>;
  }

  // ✅ Normal layout render
  return (
    <div className="flex gap-[4.4375rem] bg-black bg-no-repeat bg-center bg-cover">
      <BodyComponent readPath={readPath} session={session} />
      <div className="flex flex-col flex-1 gap-[4.25rem]">
        <div className="flex pt-[3.5rem]">
          <HeaderComponent readPath={readPath} session={session} />
        </div>
        <div className="w-[calc(100%-2.75rem)]">{children}</div>
      </div>
    </div>
  );
}
