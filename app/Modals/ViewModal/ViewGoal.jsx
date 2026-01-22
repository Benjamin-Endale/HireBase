import React from 'react'

const ViewGoal = ({per , onClose}) => {
    console.log(per)
    const handlePriority = (status) => {
        switch (status) {
            case "High": return "text-Error";  
            case "Low": return "text-lemongreen";  
            case "Medium": return "text-yellowCust";  
            default: return "text-Error"; // Default case
        }
    };

    const handleState = (status) => {
        switch (status) {
            case "InProgress": return "text-yellowCust";  
            case "Complete": return "text-lemongreen";  
        }
    };
  return (
    <div className='px-[3rem] py-[2.875rem] space-y-[3.125rem] font-semibold'>
        {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="textFormColor">{per.goalTitle}</h1>
          <h4 className="text-limegray">{per.employeeName}</h4>
        </div>
        <button
          onClick={onClose}
          className="rounded-full center-center cursor-pointer"
        >
          <img src="/image/Icon/Action/CloseCircle.png" alt="close" />
        </button>
      </div>
      <div className='space-y-14.75'>
        <div className='space-y-4.5'>
            <div className='flex justify-between'>
                <div><h4 className='textFormColor'>Description</h4></div>
                <div className='flex items-center gap-[0.5rem]'>
                    <span className={`bg-[rgba(190,229,50,0.05)] text-sm px-[20px] py-[7px] rounded-full ${handlePriority(per.priority)}`}>{per.priority || '--'}</span>
                    <span className={`bg-[rgba(190,229,50,0.05)] text-sm px-[20px] py-[7px] rounded-full ${handleState(per.status)}   `}>{per.status || "--"}</span>
                </div>
            </div>
            <div>
                <div className={` transition-all duration-500 h-50 overflow-y-scroll scrollBarDash`}>
                    <p className="text-limegray text-[15px] text-left">{per.description}</p>
                </div>
            </div>
        </div>
        <div className='space-y-6'>
            <div className='flex items-center gap-[0.75rem]'>
                <img src="/image/Icon/Action/calendarSecond.png" alt="" />
                <span className='textLimegray'>Due: {new Date(`${per.dueDate}`).toLocaleDateString("en-US", {month: "short",day: "numeric",year: "numeric",}) || "--"} </span>
            </div>
            <div className='flex items-center gap-[0.75rem]'>
                <img src="/image/Icon/Action/Bars.png" alt="" />
                <span className='textLimegray'>{per.category}</span>
            </div>
        </div>
      </div>
    </div>
  )
}

export default ViewGoal