'use client'
import React from 'react'
import { ProgressDash } from '@/app/Components/ProgressDash'
import StackedLeaveProgress from '@/app/Components/ProgressLeave'
import { useRouter } from 'next/navigation'

const page = () => {
    const router = useRouter()
  return (
    <div className='font-semibold space-y-18.5'>
        <div className='flex items-center gap-[0.9375rem]'>
            <img onClick={() => router.push('/EmployeePortal/LeaveRequest')} className='cursor-pointer' src="/image/Icon/ArrowLeft.png" alt="Back" />
            <div>
                <h4 className='textWhite'>Available Leave Credits </h4>
                <span className='textLimegray'>Employee count and attendance by department</span>
            </div>
        </div>
        <div className='space-y-17'>
            {/* HeadProgress */}
            <div className='flex flex-col gap-4.75'>
                <div className='between-center'>
                    <div className='flex items-center gap-[1.375rem]'>
                        <img src="/image/Icon/LeaveTag.png" alt="" />
                        <span className='text-formColor'>Total Leave Credits</span>
                    </div>
                    <div>
                        <ul className='text-white flex gap-[1.75rem]'>
                        <li className='textLimegray'>180 Credits</li>
                        <li className='list-disc marker:text-lemongreen textLimegray'>
                            200 Credits
                        </li>
                        </ul>
                    </div>
                </div>
                {/* <div className='w-full h-9.75 bg-limegray rounded-[5px]'>

                </div> */}
                <StackedLeaveProgress
                height="h-9.5"
                leaves={[
                    { label: "Annual Leave", used: 180, max: 200 },
                    { label: "Sick Leave", used: 200, max: 200 },
                    { label: "Maternity1 Leave", used: 125, max: 280 },
                    { label: "Dagi Leave", used: 280, max: 280 },
                    { label: "jenenew Leave", used: 280, max: 280 },
                ]}
                />


            </div>
            {/* line */}
            <div className='bg-[rgba(93,97,80,0.3)] w-full h-[1px] mt-2'></div>
            {/* DetailProgress */}  
            <div className='flex flex-col gap-10'>
              <div   className='flex flex-col gap-[1.1875rem]'>
                <div className='between-center'>
                    <div className='flex items-center gap-[1.375rem]'>
                        <img src="/image/Icon/LeaveTag.png" alt="" />
                        <span className='text-formColor'>Annual Leave</span>
                    </div>
                    <div>
                        <ul className='text-white flex gap-[1.75rem]'>
                        <li className='textLimegray'>180 Credits</li>
                        <li className='list-disc marker:text-lemongreen textLimegray'>
                            200 Credits
                        </li>
                        </ul>
                    </div>
                </div>

                {/* Progress bar */}
                <ProgressDash attended='120' maxEmp='300' low={'bg-[#E3694A]'} high={'bg-[#DFDFDF]'} medium={'bg-[#F7AB1E]'}/>
              </div>
              <div   className='flex flex-col gap-[1.1875rem]'>
                <div className='between-center'>
                    <div className='flex items-center gap-[1.375rem]'>
                        <img src="/image/Icon/LeaveTag.png" alt="" />
                        <span className='text-formColor'>Annual Leave</span>
                    </div>
                    <div>
                        <ul className='text-white flex gap-[1.75rem]'>
                        <li className='textLimegray'>180 Credits</li>
                        <li className='list-disc marker:text-lemongreen textLimegray'>
                            200 Credits
                        </li>
                        </ul>
                    </div>
                </div>

                {/* Progress bar */}
                <ProgressDash attended='180' maxEmp='200' low={'bg-[#E3694A]'} high={'bg-[#DFDFDF]'} medium={'bg-[#F7AB1E]'}/>
              </div>

            </div>
        </div>
    </div>
  )
}

export default page