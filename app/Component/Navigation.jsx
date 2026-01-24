import Link from 'next/link'
import React from 'react'

const Navigation = ({readPath}) => {
  return (
    <div className='text-white font-semibold'>
        <div className='between '>
            <div>
                <h1 className='text-formColor text-2xl'>Hire Base JOBs</h1>
            </div>
            <div className='text-navColor font-medium flex gap-6.25'> 
                <div className={`${readPath === 'NavBars/Home' || readPath === 'NavBars/HomeDetail' || readPath === 'Apply' ? 'text-formColor' : ''}`}>
                    <Link href={"/NavBars/Home"}>Home</Link>
                </div>
                <div className={`${readPath === 'NavBars/Saved' ? 'text-formColor' : ''}`}>
                    <Link href={"/NavBars/Saved"}  >Saved</Link>
                </div>
                <div className={`${readPath === 'NavBars/Applied' ? 'text-formColor' : ''}`}>
                    <Link href={"/NavBars/Applied"}  >Applied</Link>
                </div>
                <div className={`${readPath === 'NavBars/Profile' ? 'text-formColor' : ''}`}>
                    <Link href={"/NavBars/Profile"}  >Profile</Link>
                </div>
    
            </div>
        </div>
    </div>
  )
}



export default Navigation