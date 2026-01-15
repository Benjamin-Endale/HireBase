import React from 'react'

const page = () => {
  return (
    <div className=' h-full flex flex-col   font-medium '>  
        <main className='flex flex-col gap-47.5'>
        {/* Navigation Bar   */}
            <section className='flex items-center gap-11.25 mx-9.25'>
                {/* logoSection */}
                <div className='flex items-center gap-4.75'>
                    <img src='/Logo.png' alt='Logo'></img>
                    <span className='text-2xl  font-bold text-nowrap'>Hire Base</span>
                </div>
                
                {/* Nav bar */}
                <div className='flex items-center gap-3.5 '>
                    <div>
                        <img src="/miniLine.png" alt="lines" />
                    </div>
                    <nav>
                        <div className='flex   gap-3.75'>
                            <div className='Nav hover:border-mainOrange cursor-pointer'><h4>Jobs</h4></div>
                            <div className='Nav hover:border-mainOrange cursor-pointer'><h4>About</h4></div>
                            <div className='Nav hover:border-mainOrange cursor-pointer'><h4>Contact</h4></div>
                        </div>
                    </nav>

                    <div>
                        <img src="/miniLine.png" alt="" />
                    </div>
                </div>
                {/* Account */}
                <div className='flex items-center gap-10.75 -ml-1.5'>
                    <div>
                        <button className='cursor-pointer'>
                            Login
                        </button>
                    </div>
                    <div className='bg-mainOrange border-0 rounded-[3px]  text-center w-25.25 h-11.25 '>
                        <button className='text-white text-center w-full cursor-pointer h-full'>
                            Sign Up
                        </button>
                    </div>
                </div>
            </section>
            <section className='flex flex-col gap-29.75'>
                {/* FindJob */}
                <div className='w-full flex flex-col items-center  gap-16.5 '>
                        <div>
                            <div className='flex flex-col text-center items-center justify-center  gap-6'>
                                <h4 className=' text-[4rem] tracking-tight  leading-[120.9%]'>Find a Job that suits your<br />interest & skills</h4>
                                <span className='text-[rgba(0,0,0,0.55)] text-center leading-5'>Thousands of jobs in tech, design, marketing and more. Your <br /> next career move starts here.</span>
                            </div>
                        </div>
                        {/* InputField */}
                        <div className='flex items-center gap-3.5'>
                            {/* JobTitleSearch */}
                            <div className="relative w-62.75 h-12 bg-borderBack">
                            <div className="absolute z-10 pt-3.25 pl-4 pr-2">
                                    <img src="/Action/search.png" alt="search Icon" />
                                </div>
                                <input
                                    type="text"
                                    placeholder="Job title,keyword"
                                    className="w-full z-50 h-full  px-11.5 py-3.5  border placeholder:text-placeholder  bg-borderBack border-black outline-none"
                                />

                                {/* BorderFour */}
                                <div className="absolute -top-px h-0.5 left-2 right-2 bg-white"></div>
                                <div className="absolute -bottom-px h-0.5 left-2 right-2 bg-white"></div>
                                <div className="absolute -right-px  w-0.5 top-2 bottom-2 bg-white"></div>
                                <div className="absolute -left-px  w-0.5 top-2 bottom-2 bg-white"></div>
                            </div>

                            {/* Location */}
                            <div className="relative    w-62.75 h-12 bg-borderBack">
                                <div className="absolute z-10 pt-3.25 pl-4 pr-2">
                                    <img src="/Action/location.png" alt="search Icon" />
                                </div>
                                <input
                                    type="text"
                                    placeholder="Location"
                                    className="w-full z-50 h-full  px-11.5 py-3.5 border placeholder:text-placeholder   bg-borderBack border-black outline-none"
                                />

                                {/* BorderFour */}
                                <div className="absolute -top-px h-0.5 left-2 right-2    bg-white"></div>
                                <div className="absolute -bottom-px h-0.5 left-2 right-2    bg-white"></div>
                                <div className="absolute -right-px  w-0.5 top-2 bottom-2     bg-white"></div>
                                <div className="absolute -left-px  w-0.5 top-2 bottom-2     bg-white"></div>
                            </div>

                            <div className='w-39.5 h-12 bg-mainOrange border-0 rounded-[3px]'>
                                <button className='w-full h-full text-center  border-0 rounded-[3px] bg-mainOrange cursor-pointer text-white'>Search a job</button>
                            </div>
                        </div>
                </div>
                {/* Status */}
                <div className='w-full flex flex-col gap-23 justify-between bg-lightBlack '>
                    <div>
                        <img src="/longLine.png" alt="slashLines" />
                    </div>
                    <div className='  flex items-center gap-59 px-30.25 '>
                        <div className='flex items-center gap-8'>
                            < div className='relative w-15.75 h-15.75 border border-borderGray bg-borderBacksecond'>
                                    <img src="/Icons/LiveJob.png" alt="" />
                                {/* BorderFour */}
                                <div className="blackBorderHeight -top-px"></div>
                                <div className="blackBorderHeight -bottom-px"></div>
                                <div className="blackBorderWidth  -right-px"></div>
                                <div className="blackBorderWidth -left-px"></div>
                            </div>
                            <div>
                                <h4 className='text-white font-semibold text-[2rem]'>15,475</h4>
                                <span className='text-borderText' >Live Jobs</span>
                            </div>
                        </div>
                        <div className='flex items-center gap-8'>
                           < div className='relative w-15.75 h-15.75 border border-borderGray bg-borderBacksecond'>
                                <img src="/Icons/Companies.png" alt="" />
                                {/* BorderFour */}
                                <div className="blackBorderHeight -top-px"></div>
                                <div className="blackBorderHeight -bottom-px"></div>
                                <div className="blackBorderWidth  -right-px"></div>
                                <div className="blackBorderWidth -left-px"></div>
                            </div>
                            <div>
                                <h4 className='text-white font-semibold text-[2rem]'>2,513</h4>
                                <span className='text-borderText' >Companies</span>
                            </div>
                        </div>
                        <div className='flex items-center gap-8'>
                           < div className='relative w-15.75 h-15.75 border border-borderGray bg-borderBacksecond'>
                                <img src="/Icons/NewJob.png" alt="" />
                                {/* BorderFour */}
                                <div className="blackBorderHeight -top-px"></div>
                                <div className="blackBorderHeight -bottom-px"></div>
                                <div className="blackBorderWidth  -right-px"></div>
                                <div className="blackBorderWidth -left-px"></div>
                            </div>
                            <div>
                                <h4 className='text-white font-semibold text-[2rem]'>902</h4>
                                <span className='text-borderText' >New Jobs</span>
                            </div>
                        </div>
                        <div className='flex items-center gap-8 '>
                           <div className='relative w-15.75 h-15.75 border border-borderGray bg-borderBacksecond'>
                                <img src="/Icons/Candidates.png" alt="" />
                                {/* BorderFour */}
                                <div className="blackBorderHeight -top-px"></div>
                                <div className="blackBorderHeight -bottom-px"></div>
                                <div className="blackBorderWidth  -right-px"></div>
                                <div className="blackBorderWidth -left-px"></div>
                            </div>
                            <div>
                                <h4 className='text-white font-semibold text-[2rem]'>4,663</h4>
                                <span className='text-borderText' >Candidates</span>
                            </div>
                        </div>
                    </div>
                    <div>
                        <img src="/longLine.png" alt="slashlines" />
                    </div>
                </div>
            </section>
        </main>
    </div>
  )
}

export default page