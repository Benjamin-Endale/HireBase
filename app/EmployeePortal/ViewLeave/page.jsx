// app/Admin/Employees/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import ViewLeave from './ViewLeave/page';

export default async function LeaveView() {
  const session = await auth();
  const token = session?.accessToken;
  const tenantId = session?.user?.tenantId;

 

  return <ViewLeave   /> ;

}
