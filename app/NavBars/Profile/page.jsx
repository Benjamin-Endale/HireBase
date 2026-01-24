// app/Admin/EmployeeView/[employeeID]/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import ProfileView from './ProfileView/page';

export default async function EmployeesServerPage() {
 
 
  const session = await auth();
  const token = session?.accessToken;
 
   const applicantId = session?.user.id
  return <ProfileView session={session || []} applicantId={applicantId} />;
}
