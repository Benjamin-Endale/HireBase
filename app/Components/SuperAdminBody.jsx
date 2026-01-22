'use client'
import React from 'react'
import Link from 'next/link';
import DashboardIcon from '@/public/svg/DashboardSvg/Dashboard'
import Organization from '@/public/svg/DashboardSvg/Organization';
import OrganizationSetting from '@/public/svg/DashboardSvg/OrganizationSetting'
import SuperAdmin from '@/public/svg/DashboardSvg/SuperAdmin'
import Performance from '@/public/svg/DashboardSvg/Performance';


const SuperAdminBody = ({readPath}) => {
  return (
    <aside className='border customBorder scrollBar  w-[20.5rem]  h-screen flex flex-col gap-[4.25rem] relative  pt-[3.5rem] overflow-y-auto font-semibold'>
        <div className=' flex items-center gap-[1.25rem] pl-[2.75rem]'>
            <img className='w-[2.0625rem] h-[2.3125rem]' src="/image/logo.png" alt="" />
            <div >
                <h1 className='text-[1.4rem] text-white'>Hire Base Platforms</h1>
                <h4 className='text-limegray font-medium'>Job Posting Site</h4>
            </div>
        </div>
        <nav className='flex flex-col gap-[4.0625rem] overflow-y-auto scrollBar'>
            <section className='space-y-[1.5625rem] w-full  pl-[2.75rem] relative'>
                {/* All Organization */}
                <div>
                    <h4 className={`${['SuperAdmin/AllOrganization', 'SuperAdmin/CreateOrganization' ,'SuperAdmin/OrganizationSettings'].includes(readPath) ? 'text-lemongreen' : 'text-limegray'} text-[0.9375rem]`}>ORGANIZATIONS</h4>
                </div>
 
                {/* Create Organization */}
                <div className='flex items-center' >
                    <div className={`${readPath === 'SuperAdmin/CreateOrganization/CreateTenant' || readPath === "SuperAdmin/CreateOrganization/RegisterAdmin" || readPath === "SuperAdmin/CreateOrganization/UserAccess" || readPath === "SuperAdmin/CreateOrganization/Compensation" ? 'flex' : 'hidden'} absolute  left-0   navBarhover `}></div>
                    <div className='navLinkconfig'>
                        <Organization readPath={readPath}/>
                        <div>
                            <Link href='/SuperAdmin/CreateOrganization/CreateTenant'><h4 className={`${readPath === 'SuperAdmin/CreateOrganization/CreateTenant' || readPath === "SuperAdmin/CreateOrganization/RegisterAdmin" || readPath === "SuperAdmin/CreateOrganization/UserAccess" || readPath === "SuperAdmin/CreateOrganization/Compensation"  ? 'text-white' : 'text-limegray'} ` }>Create Tenant </h4></Link>
                        </div>
                    </div>
                </div>
            </section>
        </nav>
    </aside>
    )
}

export default SuperAdminBody