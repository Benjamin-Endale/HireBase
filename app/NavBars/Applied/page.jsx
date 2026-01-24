// app/Admin/Employees/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import Applied from './AppliedDetail/page';

export default async function EmployeesServerPage() {
  const session = await auth();
 
  const token = session?.accessToken;
  const applicantId = session?.user.id
     console.log("Applied: " , applicantId)
  const Appliedjobs = await hrmsAPI.getApplicantApply(applicantId)
    console.log("Applied: " , Appliedjobs)
  return <Applied Appliedjobs={Appliedjobs} /> ;

}
