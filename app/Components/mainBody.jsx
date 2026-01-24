'use client';

import React from 'react';
import Link from 'next/link';
import Recruitment from '@/public/svg/DashboardSvg/Recruitment';

const MainBody = ({ readPath }) => {
  const menuSections = [
    {
      title: 'TALENT',
      items: [
        {
          path: '/Admin/RecruitmentPages/Jobposting',
          label: 'Recruitment',
          icon: Recruitment,
        },
      ],
    },
  ];

  const isActive = (item) =>
    readPath === item.path.replace('/', '') ||
    (
      item.path === '/Admin/RecruitmentPages/Jobposting' &&
      [
        'Admin/RecruitmentPages/Shortlist',
        'Admin/RecruitmentPages/Candidates',
        'Admin/RecruitmentPages/Interviews',
      ].includes(readPath)
    );

  return (
    <aside className="font-semibold customBorder scrollBar w-[20.5rem] h-screen flex flex-col gap-[4.25rem] relative pt-[3.5rem] overflow-y-auto">
      {/* Header */}
      <div className="flex items-center gap-[1.25rem] pl-[2.75rem]">
        <img
          className="w-[2.0625rem] h-[2.3125rem]"
          src="/image/logo.png"
          alt="Logo"
        />
        <div>
          <h1 className="text-[1.4rem] text-white font-semibold">
            HRMS Platforms
          </h1>
          <h4 className="text-limegray">Multi-Tenant HR System</h4>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex flex-col gap-[4.0625rem] overflow-y-auto scrollBar">
        {menuSections.map((section) => (
          <section
            key={section.title}
            className="space-y-[1.5625rem] w-full pl-[2.75rem] relative"
          >
            <h4
              className={`${
                section.items.some(isActive)
                  ? 'text-lemongreen'
                  : 'text-limegray'
              } text-[0.9375rem]`}
            >
              {section.title}
            </h4>

            {section.items.map((item) => {
              const Icon = item.icon;

              return (
                <div key={item.path} className="flex items-center">
                  <div
                    className={`${
                      isActive(item) ? 'flex' : 'hidden'
                    } absolute left-0 navBarhover`}
                  />
                  <div className="navLinkconfig">
                    <Icon readPath={readPath} />
                    <div>
                      <Link href={item.path}>
                        <h4
                          className={`${
                            isActive(item)
                              ? 'text-white'
                              : 'text-limegray'
                          }`}
                        >
                          {item.label}
                        </h4>
                      </Link>
                    </div>
                  </div>
                </div>
              );
            })}
          </section>
        ))}
      </nav>
    </aside>
  );
};

export default MainBody;
