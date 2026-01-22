'use client'
import React , {useState} from 'react'
import EmployeePortalSubNav from '@/app/EmployeePortalSubNav'
import { Dropdown } from '@/app/Components/DropDown'
import { hrmsAPI } from '@/app/lib/api/client'



const page = ({goals , userId , token}) => {
 
    const [selectedPercent, setSelectedPercent] = useState(Array(goals.length).fill(null))
    const options = Array.from({ length: 101 }, (_, i) => `${i}%`)


const handleProgress = async (goalId, progress) => {
  try {
    console.log(goalId)
    const result = await hrmsAPI.updateProgress(goalId, progress, token);
    console.log("Updated:", result);
  } catch (err) {
    console.error("❌ Error Updating Progress:", err.message || err);
  }
};



      
  return (
    <>
    <EmployeePortalSubNav readPath= '/EmployeePortal/Performance/Goals'/>
    <div className='font-semibold my-[3.25rem] h-screen'>
      <div className='flex gap-[5.4375rem] flex-wrap'>
        {goals.length != 0  ? (goals.map((task, idx) => (
          <React.Fragment key={idx}>
            <div className='space-y-[2rem] w-[300px]'>
              <div>
                <div className='mb-[0.9375rem]'>
                    <span className='bg-[rgba(190,229,50,0.05)] text-sm px-[20px] py-[8px] rounded-full text-Error'>{task.priority}</span>
                </div>
                <div className='space-y-[1.0625rem]'>
                  <h1 className='text-formColor'>{task.goalTitle}</h1>
                  <h4 className='textLimegray w-[20rem] text-wrap '>{task.description}</h4>
                </div>
              </div>
              <div className='space-y-[0.625rem]'>
                <div className='flex gap-[0.75rem]'>
                  <img src="/image/Icon/Action/calendarSecond.png" alt="" />
                  <h4 className='text-limeLight font-medium'>Due: {new Date(`${task.dueDate}`).toLocaleDateString("en-US", {month: "short",day: "numeric",year: "numeric",}) || "--"} </h4>
                </div>
                <div className='flex gap-[0.75rem]'>
                  <img src="/image/Icon/Action/Bars.png" alt="" />
                  <h4 className='text-limeLight font-medium'>{task.category}</h4>
                </div>
              </div>

              <div>
                <Dropdown
                  label=""
                    options={options}
                    selected={selectedPercent[idx]}
                    onSelect={(val) => {
                        const newPercents = [...selectedPercent];
                        newPercents[idx] = val;
                        setSelectedPercent(newPercents);

                        // Call API with the correct goalId and integer progress
                        const progressNumber = parseInt(val.replace("%","")); 
                        handleProgress(task.id, progressNumber);
                    }}

                  placeholder="Select Progress"
                  className="w-[18.6875rem]"
                  color="bg-[rgba(13,15,17,1)]"
                  ClassForborder="bg-inherit w-full h-[3.125rem] placeholder-input text-formColor rounded-[10px] pl-[1.1875rem] border border-[rgba(88,88,88,1)]"
                />
              </div>
            </div>
          </React.Fragment>
        ))) : (
            <div className='w-full'>
                <h4 className='center-center text-lemongreen'>No Tasks Currently</h4>
            </div>
        )}

      </div>

    </div>
    </>
  )
}

export default page