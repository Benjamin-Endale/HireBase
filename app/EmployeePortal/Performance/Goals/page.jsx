// app/Admin/EmployeeView/[employeeID]/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import Goal from './GoalView/page';

export default async function EmployeesServerPage( ) {
 
 

  const session = await auth();
  const token = session?.accessToken;
  const tenantId = session?.user.tenantId
 
   const userId = session?.user.id
 
  const goals = await hrmsAPI.getGoalbyEmployeeId(userId, token);
 
 
    console.log("🧾 goals fetched from backend:", goals);
 
 
  return <Goal  goals={goals} userId={userId} token={token} tenantId={tenantId}      />;
}
  