// app/Admin/Employees/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import Saved from './SavedView/page';

export default async function EmployeesServerPage() {
  const session = await auth();
  const token = session?.accessToken;
  const applicantId = session?.user.id
 
  const savedJob = await hrmsAPI.getsavedJobs(applicantId)
  console.log(savedJob)
  return <Saved    savedJob={savedJob}   token={token}/> ;

}