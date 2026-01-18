import React from 'react'

const page = () => {
  return (
    <div className='flex flex-col min-h-dvh font-medium mb-20 px-29.5'>
        <main>
            <div className='relative border-2 flex flex-col gap-47.5  border-t-0 pt-11.5 pb-39.75  border-borderDark '>
                <div className='absolute  w-0.5 h-full bg-borderBack -left-0.5 bottom-2'></div>
                <div className='absolute  w-0.5 h-full bg-borderBack -right-0.5 bottom-2'></div>
                <div className='absolute   h-0.5 -bottom-0.5  bg-borderBack left-2 right-2  '></div>

                {/* Navigation Bar   */}
                <section className='flex   items-center gap-11.25 mx-9.25'>
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
                    <div className='flex items-center gap-10.75  -ml-1.5'>
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
                <section className='flex flex-col gap-29.75  '>
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
                    <div className='w-full h-87.75 flex flex-col gap-23 justify-between bg-lightBlack '>
                        <div>
                            <img src="/longLine.png" alt="slashLines" />
                        </div>
                        <div className='flex items-center gap-59 px-30.25 '>
                            <div className='flex items-center gap-8'>
                                <div className='relative w-15.75 h-15.75 border border-borderGray bg-borderBacksecond'>
                                    <div className='relative w-full h-full'>
                                        <img className='absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2' src="/Icons/LiveJob.png" alt="" />
                                    </div>
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
                            <div className='relative w-15.75 h-15.75 border border-borderGray bg-borderBacksecond'>
                                <div className='w-full h-full relative'>
                                    <img className='absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2' src="/Icons/Companies.png" alt="" />
                                </div>
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
                            <div className='relative w-15.75 h-15.75 border border-borderGray bg-borderBacksecond'>
                                    <div className='w-full h-full relative'>
                                        <img className='absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2'  src="/Icons/NewJob.png" alt="" />
                                    </div>
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
                                    <div className='absolute w-full h-full'>
                                        <img className='absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2'  src="/Icons/Candidates.png" alt="" />
                                    </div>
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

                    <div className='ml-35.5   w-199.5 flex flex-col gap-4'>
                        <div className='flex items-center gap-3.75'>
                            <div className='w-2.5 h-2.5 bg-mainOrange '></div>
                            <h4 className='text-[1.25rem] text-nowrap'>60% of job <span className='text-lightText text-[1.25rem] text-nowrap'>listings on the internet are outdated, spammy or no longer hiring.</span> </h4>
                        </div>
                        <div className='tracking-[139%]'>
                            <h1 className='text-[2rem]'>HireBase <span className='text-lightText text-[2rem]'>scans the internet</span> to find real jobs <br />directly from companies that are actually hiring <br /> now.</h1>
                        </div>
                    </div>
                </section>
            </div>
                <div className='my-4'>
                    <img src="/whiteLongLine.png" alt="slash" />
                </div>
                {/* SecondPart */}
                <section className='relative border-2  py-40.75  border-borderDark'>
                    <div className='absolute  w-0.5   top-2 bg-borderBack -left-0.5 bottom-2'></div>
                    <div className='absolute  w-0.5   top-2 bg-borderBack -right-0.5 bottom-2'></div>
                    {/* BigBlackBorder */}
                    <div className='absolute  w-10.75 h-full top-0 bg-black right-0 bottom-0'></div>

                    <div className='absolute   h-0.5 -bottom-0.5  bg-borderBack left-2 right-2  '></div>
                    <div className='absolute   h-0.5 -top-0.5  bg-borderBack left-2 right-2  '></div>

                    {/* Content */}
                    <div className='flex   h-full '>
                        <div className='h-full   ml-35 mr-28.25  flex flex-col    justify-center gap-10'>
                            <div className='flex flex-col gap-5.25'>
                                <div>
                                    <img src="/Icons/zap.png" alt="" />
                                </div>
                                <div className='space-y-5.25'>
                                    <h4 className='text-[2rem] tracking-tight leading-[139%] font-semibold'>Find Your Next  <br /> Opportunity  <span className='italic'>Faster</span></h4>
                                    <p className='text-lightText w-full   leading-[139%]  '>Discover jobs, leads, or business opportunities with real-time  data <br /> straight from companies. Be among the first to apply, connect, or <br /> pitch—so you stay ahead of the competition.</p>
                                </div>
                            </div>
                            <button className='w-39.25 h-12 bg-mainOrange flex items-center justify-center rounded-[3px] gap-2.5 cursor-pointer'>
                                <span className='text-white'>Explore Job</span>
                                <img src="/Icons/arrowRight.png" alt="" />
                            </button>

                            {/* InfiniteScroll */}
                <div className="relative w-111.25 h-6.5 border border-red-900 overflow-hidden">
                <div className="absolute left-0 top-0 flex w-max items-center gap-8 animate-marquee whitespace-nowrap">
                                    <h4 className="text-black shrink-0">Google</h4>
                                    <h4 className="text-black shrink-0">OpenAI</h4>
                                    <h4 className="text-grayWhite shrink-0">HireBase</h4>
                                    <h4 className="text-black shrink-0">Google</h4>
                                    <h4 className="text-grayWhite shrink-0">HireBase</h4>
                                    <h4 className="text-black shrink-0">OpenAI</h4>
                                    <h4 className="text-black shrink-0">Google</h4>
                                    <h4 className="text-black shrink-0">OpenAI</h4>
                                    <h4 className="text-grayWhite shrink-0">HireBase</h4>
                                    <h4 className="text-black shrink-0">Google</h4>
                                    <h4 className="text-grayWhite shrink-0">HireBase</h4>
                                    <h4 className="text-black shrink-0">OpenAI</h4>
                                    <h4 className="text-black shrink-0">Google</h4>
                                    <h4 className="text-black shrink-0">OpenAI</h4>
                                    <h4 className="text-grayWhite shrink-0">HireBase</h4>
                                    <h4 className="text-black shrink-0">Google</h4>
                                    <h4 className="text-grayWhite shrink-0">HireBase</h4>
                                    <h4 className="text-black shrink-0">OpenAI</h4>
                                </div>
                            </div>


                        </div>
                        <div className='w-0.5  bg-darkenBorder'></div>
                        <div className='flex flex-1    items-center justify-center '>
                            <img src="/pages.png" alt="" />
                        </div>
                    </div>

                </section>
        </main>
    </div>
  )
}

export default page