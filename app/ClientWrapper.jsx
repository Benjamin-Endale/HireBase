'use client';

import { usePathname, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import Navigation from './Component/Navigation';
export default function ClientWrapper({ children ,session}) {
    const router = useRouter();
    const pathname = usePathname() || '/';
    const readPath = pathname === '/' ? "NavBars/Home" : pathname.replace('/', '');

    const requiresOtp = session?.requiresOtp;
    const otpVerified = session?.otpVerified;
  // ✅ Always run all hooks
  useEffect(() => {
    if (requiresOtp && !otpVerified && !pathname.startsWith('/Login/VerifyOtp')) {
      router.push(`/Login/VerifyOtp?email=${session?.id || session?.email}`);
    }
  }, [requiresOtp, otpVerified, pathname, router, session]);

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token || !session?.id || (requiresOtp && !otpVerified)) return;

    const updateLastLogin = async () => {
      try {
        await hrmsAPI.touchLogin(session.id);
      } catch (err) {
        console.error('Failed to update last login:', err);
      }
    };
    updateLastLogin();
  }, [session?.id, requiresOtp, otpVerified]);

    useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token || !session?.id || (requiresOtp && !otpVerified)) return;

    const updateLastLogin = async () => {
      try {
        await hrmsAPI.touchLogin(session.id);
      } catch (err) {
        console.error('Failed to update last login:', err);
      }
    };
    updateLastLogin();
  }, [session?.id, requiresOtp, otpVerified]);

  if (requiresOtp && !otpVerified) {
    if (pathname.startsWith('/Login/VerifyOtp')) {
      return <div>{children}</div>;
    }
    return <div>Redirecting to OTP verification...</div>;
  }

    return (
        <div>
            <div className='pt-17 px-[11.42rem] w-screen overflow-y-auto overflow-x-hidden scrollBarDash h-screen  '>
                <div className='w-[96.250001729029575rem] h-screen border '> 
                    <Navigation readPath={readPath}/>
                    <div className='mt-[5.196875rem] '>
                        {children}
                    </div>
                </div>
                    <footer className="text-white between mt-62 pb-26.25  "><h4>2025 Onyx Technologies. All rights reserved.</h4><h4>Privacy and policy </h4></footer>

            </div>
        </div>
    );
}
