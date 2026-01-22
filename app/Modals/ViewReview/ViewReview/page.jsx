'use client'
import React, {useState} from 'react'

const ViewReview = ({onClose ,reviews}) => {

    
 
  const [openId, setOpenId] = useState(null);
  const [openQuestion, setOpenQuestion] = useState({
    reviewId: null,
    questionId: null
  });
  
  const toggle = (id) => {
      setOpenId(prev => (prev === id ? null : id))
  }

  
  
  const toggleQuestion = (reviewId, questionId) => {
    setOpenQuestion(prev =>
      prev.reviewId === reviewId && prev.questionId === questionId
        ? { reviewId: null, questionId: null }
        : { reviewId, questionId }
    );
  };
  return (
    <>
    <div className='px-[3rem] py-[2.875rem] space-y-[3.125rem] font-semibold'>
      <div className="flex justify-between items-center">
        <div>
          <h1 className="textFormColor"></h1>
          <h4 className="text-limegray"> </h4>
        </div>
        <button
          onClick={onClose}
          className="rounded-full center-center cursor-pointer"
        >
          <img src="/image/Icon/Action/CloseCircle.png" alt="close" />
        </button>
      </div>
      {reviews.performancereviewemployee.length != 0 ? (reviews.performancereviewemployee.map((rev,idx)=>(
        <div key={rev.reviewId} className='border border-[rgba(88,88,88,1)] rounded-[20px] px-[2.1875rem] py-[2rem] space-y-5 '>
            <div className='space-y-[1.625rem]'>
                <div className='flex justify-between items-center'>
                    <div className='flex gap-7'>
                        <h1 className='text-formColor'>Overall Feedback</h1>
                        <li className='list-disc marker:text-lemongreen text-limegray'>{rev.createdAt ? new Date(rev.createdAt).toLocaleDateString('en-GB').replace(/\//g,'-') : '--|--'}</li>
                    </div>
                    <div>
                        <button onClick={()=>toggle(rev.reviewId)}><img src="/image/Icon/ArrowDown.png" className={`${openId === rev.reviewId ? 'transition-all rotate-180 duration-500' : 'duration-500 transition-all'}`} alt="" /></button>
                    </div>
                </div>
            <div   className={`overflow-hidden transition-all duration-500 ease-in-out overflow-y-scroll scrollBarNav  ${openId === rev.reviewId ? '  opacity-100 max-h-[15rem]' : ' transition-all duration-500 max-h-0 opacity-0'}`}>
                <p  className={`textLimegray `}>{rev.overallFeedback}.</p>
            </div>
            </div>
            {rev.questions.map((feed,idx)=>(
                <div key={feed.reviewQuestionId} className='space-y-[1.625rem]'>
                    <div className='flex justify-between items-center'>
                        <div className='flex gap-7'>
                            <h1 className='text-formColor'>{feed.question}</h1>
                            <div className='flex items-center gap-[0.40625rem]'><img src="/image/star.png" alt="" /><span className='text-limegray'>{feed.rating}</span></div>
                        </div>
                        <div>
                            <button onClick={()=>toggleQuestion(rev.reviewId, feed.reviewQuestionId)}><img src="/image/Icon/ArrowDown.png" className={`${openQuestion.reviewId === rev.reviewId && openQuestion.questionId === feed.reviewQuestionId ? 'transition-all rotate-180 duration-500' : 'duration-500 transition-all'}`} alt="" /></button>
                        </div>
                    </div>
                <div   className={`overflow-hidden transition-all duration-500 ease-in-out   overflow-y-scroll scrollBarNav ${openQuestion.reviewId === rev.reviewId && openQuestion.questionId === feed.reviewQuestionId  ? 'max-h-[15rem] opacity-100' : ' transition-all duration-500 max-h-0 opacity-0'}`}>
                    <p className={`textLimegray`}>{feed.feedback}.</p>
                </div>
                </div>
            ))}
        </div>
      ))) : (
        <div>
            <h4 className='text-lemongreen'>
                No Reviews For Now!!
            </h4>
        </div>
      )}
    </div>
    </>
    )
 }

export default ViewReview