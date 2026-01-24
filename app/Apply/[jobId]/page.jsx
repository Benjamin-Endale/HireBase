// app/Admin/Employees/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import Apply from './ApplyView/page';

export default async function EmployeesServerPage({params}) {
    const resolvedParams = await params; // await it
  const { jobId } = resolvedParams;

  const session = await auth();
  const token = session?.accessToken;
  const applicantId = session?.user.id
 console.log("This is my JOb iD: ",jobId)
 
  return <Apply   applicantId={applicantId} jobID={jobId}  token={token}/> ;

}
