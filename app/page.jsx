import { redirect } from 'next/navigation';
import { auth } from '@/app/auth'; // server-side auth

export default async function Home() {
  const session = await auth();

  if (!session) {
    redirect('/Login');
  }

  const role = session.user.role;

  switch (role) {
    case 'SuperAdmin':
      redirect('/SuperAdmin/CreateOrganization/CreateTenant');
    case 'HR':
      redirect('/Admin/Dashboard');
    case 'SystemAdmin':
      redirect('/Admin/RecruitmentPages/Jobposting');
    case 'Employee':
      redirect('/EmployeePortal/Dashboard');
    default:
      redirect('/Login'); // fallback
  }

}
