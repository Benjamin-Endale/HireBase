'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import DashboardIcon from '@/public/svg/DashboardSvg/Dashboard';
import Organization from '@/public/svg/DashboardSvg/Organization';
import Employee from '@/public/svg/DashboardSvg/Employee';
import Attendance from '@/public/svg/DashboardSvg/Attendance';
import LeaveManegment from '@/public/svg/DashboardSvg/LeaveManegment';
import Recruitment from '@/public/svg/DashboardSvg/Recruitment';
import Performance from '@/public/svg/DashboardSvg/Performance';
import Training from '@/public/svg/DashboardSvg/Training';
import Announcement from '@/public/svg/DashboardSvg/Announcement';
import UserAuth from '@/public/svg/DashboardSvg/UserAuth';
import Settings from '@/public/svg/DashboardSvg/Setting';
import Department from '@/public/svg/DashboardSvg/Department';
import { Dropdown } from '@/app/Components/DropDownforNav'; // make sure path is correct
import { hrmsAPI } from '@/app/lib/api/client';
import { useRouter } from 'next/navigation';
import OrgSelectorPanel from './OrgSelectorPanel';


const MainBody = ({ readPath, modules, session }) => {
  const [organizations, setOrganizations] = useState([]);
  const [selectedOrg, setSelectedOrg] = useState('');
  const [loading, setLoading] = useState(true);
  const tenantId = session?.user?.tenantId;
  const token =  session?.accessToken
  const router = useRouter()

  useEffect(() => {
    const fetchOrganizations = async () => {


      try {
        
        const res = await hrmsAPI.getOrganizationsByTenantId(tenantId, token);
        setOrganizations(res || []);
        if (res?.length > 0) setSelectedOrg(res[0].name);
      } catch (err) {
        console.error('Error fetching organizations:', err);
      } finally {
        setLoading(false);
      }
    };

    if (tenantId && token) {
      fetchOrganizations();
    }
  }, [tenantId, token]);


  const handleOrgSelect = (orgName) => {
  const org = organizations.find((o) => o.name === orgName);
  if (!org) return;

  setSelectedOrg(orgName);

  console.log('Switched to organization:', org);

  // Navigate to the organization dashboard using its ID
  router.push(`/Admin/OrganizationPages/${org.id}`);
};


  const menuSections = [
 
    {
      title: 'TALENT',
      items: [
        { path: '/Admin/RecruitmentPages/Jobposting', label: 'Recruitment', icon: Recruitment, moduleKey: 'recruitment' },
      ],
    },
  ];

  const isActive = (item) =>
    readPath === item.path.replace('/', '') ||
    (item.path === '/Admin/RecruitmentPages/Jobposting' &&
      ['Admin/RecruitmentPages/Shortlist', 'Admin/RecruitmentPages/Candidates', 'Admin/RecruitmentPages/Interviews' ].includes(readPath))  


  return (
    <aside className="font-semibold customBorder scrollBar w-[20.5rem] h-screen flex flex-col gap-[4.25rem] relative pt-[3.5rem] overflow-y-auto">
      {/* Header */}
      <div className="flex items-center gap-[1.25rem] pl-[2.75rem]">
        <img className="w-[2.0625rem] h-[2.3125rem]" src="/image/logo.png" alt="Logo" />
        <div>
          <h1 className="text-[1.4rem] text-white font-semibold">HRMS Platforms</h1>
          <h4 className="text-limegray">Multi-Tenant HR System</h4>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex flex-col gap-[4.0625rem] overflow-y-auto scrollBar">
        {menuSections.map((section) => {
          const hasVisibleItems = section.items.some(
            (item) => !item.moduleKey || modules?.[item.moduleKey] === true
          );
          if (!hasVisibleItems) return null;

          return (
            <section key={section.title} className="space-y-[1.5625rem] w-full pl-[2.75rem] relative">
              <h4 className={`${section.items.some(isActive) ? 'text-lemongreen' : 'text-limegray'} text-[0.9375rem]`}>
                {section.title}
              </h4>

              {section.items.map((item) => {
                const Icon = item.icon;
                const visible = !item.moduleKey || modules?.[item.moduleKey] === true;
                if (!visible) return null;

                return (
                  <div key={item.path} className="flex items-center">
                    <div className={`${isActive(item) ? 'flex' : 'hidden'} absolute left-0 navBarhover`} />
                    <div className="navLinkconfig">
                      <Icon readPath={readPath} />
                      <div>
                        <Link href={item.path}>
                          <h4 className={`${isActive(item) ? 'text-white' : 'text-limegray'}`}>{item.label}</h4>
                        </Link>
                      </div>
                    </div>
                  </div>
                );
              })}
            </section>
          );
        })}
 

      </nav>
    </aside>
  );
};

export default MainBody;

 