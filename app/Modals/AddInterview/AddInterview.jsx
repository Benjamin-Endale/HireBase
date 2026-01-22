'use client';

import React, { useState , useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Dropdown } from '@/app/Components/DropDown';
import { useForm, Controller } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import DropDownSearch from '@/app/Components/DropDownSearch'
import { hrmsAPI } from '@/app/lib/api/client';


const scheduleSchema = z.object({
  applicantEmail: z.string().email("Invalid candidate email"),
 
  scheduledDate: z.string()
  .min(1, 'Date is required')
  .refine(
      (value) => {
        if (!value) return false;
        const today = new Date();
        const date = new Date(value);
        return date >= today;
      },
      { message: 'Date must be today or later' }
    ),
  scheduledTime: z.string().min(1, "Time required"),

  duration: z.string().min(1, "Duration is required"),
  locationOrMeetingUrl: z.string().min(1, "Location or Meeting URL required"),

  interviewerEmail: z.string().email("Invalid interviewer email"),
  interviewNote: z.string().optional(),
  mode: z.string().min(1, "Mode is required"),
});


export default function AddInterview({ onClose ,   ShortlIst , users }) {
 
 
    const [selectedCandidate, setSelectedCandidate] = useState('');
    const [selectedMode, setSelectedMode] = useState('');
    const [selectedInterview, setSelectedInterview] = useState('');
    const [isSubmitting , setIsSubmitting] = useState(false)
    const [time, setTime] = useState("");
 
  

 

  const {
    register,
    setValue,
    handleSubmit,
    control,
    formState: { errors }
  } = useForm({
    resolver: zodResolver(scheduleSchema),
    defaultValues: {
      applicantEmail: "",
      scheduledDate: "",
      scheduledTime: "",
      duration: "",
      locationOrMeetingUrl: "",
      interviewerEmail: "",
      interviewNote: "",
      mode: "",
    },
  });
 


 
 
const onSubmit = async (data) => {
  try {
    // Get the full candidate using the selected email
    const selected = ShortlIst.shortlistedApplicants.find(
      (x) => x.email === data.applicantEmail
    );

    const interviewPayload = {
      ...data,
      jobTitle: selected?.jobTitle || null,
    };

    const result = await hrmsAPI.createInterviewfromShortlist(interviewPayload);
    onClose();
  } catch (err) {
    console.error("❌ Error saving Schedule:", err.message || err);
  } finally {
    setIsSubmitting(false);
  }
};


 
  return (
    <div className="px-[3rem] py-[2.875rem] space-y-[3.125rem] font-semibold w-full">
      {/* Header */}
      <div className="flex justify-between">
        <div>
          <h1 className="textFormColor">Schedule New Interview</h1>
          <h4 className="text-limegray">Set up an interview with a candidate</h4>
        </div>
        <button
          onClick={onClose}
          className="rounded-full center-center cursor-pointer"
        >
          <img src="/image/Icon/Action/CloseCircle.png" alt="close" />
        </button>
      </div>

      {/* Form */}
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="flex flex-col gap-[2.375rem]"
      >
 
        <div className="w-full flex gap-[1.125rem]">
          {/* Left side */}
          <div className="w-[15.5625rem] flex flex-col gap-[2.375rem]">
            {/* Candidate */}
            <Controller
              name="applicantEmail"
              control={control}
              render={({ field }) => (
              <DropDownSearch
                label="Candidate"
                options={ShortlIst.shortlistedApplicants.map((s) => ({
                  label: (
                    <div>
                      <div className="font-medium">{s.name}</div>
                      <div className="text-sm text-limegray">{s.email}</div>
                    </div>
                  ),
                  labelText: `${s.name} ${s.email}`, // for search
                  value:   s.email,
                  displayName:    s.name,
                    original: s        
                }))}
  
                selected={selectedCandidate}
                onSelect={(email, option) => {
                setSelectedCandidate(option.displayName);  // UI shows name
                field.onChange(email);                    // backend gets email
                }}

                placeholder="Search Candidate..."
                 
              />

              )}
            />
            {errors.applicantEmail && (
              <p className="text-Error text-[1rem]">
                {errors.applicantEmail.message}
              </p>
            )}

            {/* Mode */}
            <Controller
              name="mode"
              control={control}
              render={({ field }) => (
                <Dropdown
                  label="Mode"
                  options={['Online', 'In Person', ]}
                  selected={selectedMode}
                  onSelect={(value) => {
                    setSelectedMode(value);
                    field.onChange(value);
                  }}
                  placeholder="Select Mode"
                   
                />
              )}
            />
            {errors.mode && (
              <p className="text-Error text-[1rem]">{errors.mode.message}</p>
            )}

          <div className='flex flex-col gap-[1rem] relative'>
            <label className='text-formColor'>Time</label>
            <Controller
              name="scheduledTime"
              control={control}
              render={({ field }) => (
                <input
                  type="time"
                  className="inputMod pr-[1.1875rem]"
                  value={time}
                  onChange={(e) => {
                    setTime(e.target.value);
                    field.onChange(e.target.value);
                  }}
                />
              )}
            />

          </div>
          
          </div>

          {/* Right side */}
          <div className="w-[15.5625rem] flex flex-col gap-[2.375rem]">
            {/* Interviewer */}
            <Controller
              name="interviewerEmail"
              control={control}
              render={({ field }) => (
              <DropDownSearch
                label="Interviewer"
                options={users.map((u) => ({
                  label: (
                    <div>
                      <div className="font-medium">{u.fullName}</div>
                      <div className="text-sm text-limegray">{u.email}</div>
                    </div>
                  ),
                  labelText: `${u.fullName} ${u.email}`, // for search
                  value: u.email,
                  displayName: u.fullName                 
                }))}
  
                selected={selectedInterview}
                onSelect={(email , option) => {
                  setSelectedInterview(option.displayName);
                  field.onChange(email);
                }}
                placeholder="Search interviewer..."
                 
              />

              )}
            />
            {errors.interviewerEmail && (
              <p className="text-Error text-[1rem]">
                {errors.interviewerEmail.message}
              </p>
            )}

 
            {/* Duration */}
            <div className="flex flex-col w-full gap-[1rem]">
              <label className="textFormColor1">duration</label>
              <input type="number" placeholder='20'  className="inputMod pr-[1.5625rem]" {...register('duration')} />
              {errors.duration && <p className="text-Error text-[1rem]">{errors.duration.message}</p>}
            </div>
 

            {/* Schedule Date */}
            <div className="flex flex-col w-full gap-[1rem]">
              <label className="textFormColor1">Schedule Date</label>
              <input type="date" className="inputMod pr-[1.5625rem]" {...register('scheduledDate')} />
              {errors.scheduledDate && <p className="text-Error text-[1rem]">{errors.scheduledDate.message}</p>}
            </div>
 
          </div>
        </div>

        {/* Location / Note */}
        <div>
          <div className="flex flex-col gap-[2.375rem]">
            <div className="flex flex-col gap-[1rem]">
              <label className="text-formColor">Location / Meeting Link</label>
              <textarea
                placeholder="Conference Room or Zoom link"
                className="text-formColor bg-inputBack rounded-[10px] placeholder-input pt-[0.59375rem] pl-[1.1875rem] resize-none h-[5.5rem]"
                {...register('locationOrMeetingUrl')}
              />
              {errors.locationOrMeetingUrl && (
                <p className="text-Error text-[1rem]">
                  {errors.locationOrMeetingUrl.message}
                </p>
              )}
            </div>
 
            {/* Interview Note */}
            <div className="flex flex-col gap-[1rem]">
              <label className="text-formColor">Interview Note</label>
              <textarea
                placeholder="Add any special instructions or notes"
                className="text-formColor bg-inputBack rounded-[10px] placeholder-input pt-[0.59375rem] pl-[1.1875rem] resize-none h-[5.5rem]"
                {...register('interviewNote')}
              />
              {errors.interviewNote && (
                <p className="text-Error text-[1rem]">{errors.interviewNote.message}</p>
              )}
            </div>

          </div>
        </div>

        {/* Submit Button */}
        <div className="w-full h-[3.4375rem] mt-[0.5rem]">
          <button
            type="submit"
            className="w-full h-full bg-lemongreen rounded-[10px] cursor-pointer"
            onClick={()=>setIsSubmitting(true)}
          >
            {isSubmitting ? 'Scheduling...' : 'Schedule'}
          </button>
        </div>
      </form>
    </div>
  );
}
