// app/Admin/Employees/page.jsx
import { auth } from '@/app/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import ReviewView from './ViewReview/page';

export default async function ReviewQuestionsPage({id}) {
  const session = await auth();
  const token = session?.accessToken;
  const tenantId = session?.user?.tenantId;
 
 
  console.log(id)
 
  const reviews = await hrmsAPI.getReviewByuserID(id, token);
  console.log(reviews)
  return <ReviewView reviews={reviews} /> ;

}
