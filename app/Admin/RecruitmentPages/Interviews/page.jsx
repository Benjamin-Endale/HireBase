// app/Admin/EmployeeView/[employeeID]/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import Interview from './interviewView/page';

export default async function EmployeesServerPage( ) {
 
 

  const session = await auth();
  const token = session?.accessToken;
  console.log(session)
  const tenantId = session?.user.tenantId
 
  const users = await hrmsAPI.getUser(tenantId, token);
 
  const ShortlIst = await hrmsAPI.getShortList(tenantId, token);
  const jobs  = await hrmsAPI.getJobs(tenantId,token)
  const Applicants = await hrmsAPI.getApplicant(tenantId, token);
  const interviews = await hrmsAPI.getInterview(token)
  console.log("🧾 interviews fetched from backend:", interviews);

  return <Interview  jobs={jobs}  interviews={interviews} users={users}  ShortlIst={ShortlIst} Applicants={Applicants}/>;
}
  