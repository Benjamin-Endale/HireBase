// app/Admin/Employees/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import Home from './HomeView/page';

export default async function EmployeesServerPage() {
  const session = await auth();
  const token = session?.accessToken;
  const applicantId = session?.user.id
  const jobs = await hrmsAPI.getJobs()
  const savedJob = await hrmsAPI.getsavedJobs(applicantId)
 
  return <Home jobs={jobs || []}  savedJob={savedJob || []} applicantId={applicantId || []} token={token}/> ;

}
