import React from 'react';
import { logout } from '@/app/lib/actions/auth';

export default function Signout({ Value }) {


  return (
    <div className=' flex-col center-center gap-4 px-12 py-11.5 font-semibold'>
      <div className='flex-col center-center  mb-2 '>
        <h4 className='text-formColor'>Your {Value} is Changed!</h4>
        <h4 className='text-limegray'>Logout to see Change</h4>
      </div>
      <div className='flex gap-5'>
        <div  className='rounded-[10px] center-center bg-lemongreen w-37.5 text-black h-13.75'>
            <button onClick={()=>logout()}  className='w-full h-full cursor-pointer'>Log Out</button>
        </div>

      </div>
    </div>
  );
}
