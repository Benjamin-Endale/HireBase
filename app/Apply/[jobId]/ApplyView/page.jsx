'use client'
import { useRouter } from 'next/navigation'
import React, { useState } from 'react'
import { useForm, Controller } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
 import { hrmsAPI } from '@/app/lib/api/client';
import { toPascal } from '@/app/lib/utils/toPascal';

const Schema = z.object({
    Name: z.string().min(2, 'Name is required'),
    Phone: z.string().min(10, 'Phone Number is required'),
    Email: z.string().email('Invalid email address'),
    ResumeUrl: z
    .any()
    .refine((file) => file && file.length > 0, 'Upload Cv is required'),
    });
const page = ({applicantId , jobID}) => {
        const router = useRouter()
        // const [isSubmitting, setIsSubmitting] = useState(false)
        const {
            register,
            handleSubmit,
            formState: { errors, isSubmitting }
        } = useForm({
            resolver: zodResolver(Schema),
            defaultValues: {
            Name: '',
            Phone: '',
            Email: '',
            ResumeUrl: '',
            },
        });
    const handleApply = async (data) => {
        try {
        console.log('Register data:', data);
        // Debug file data
        console.log("Upload file:", data.ResumeUrl)
 
 
        
        if (data.ResumeUrl instanceof FileList) {
            console.log("ResumeUrl files count:", data.ResumeUrl.length)
            console.log("First certification file:", data.ResumeUrl[0])
        }
 

        // Create FormData to see actual file contents
        const formData = new FormData()
        
        // Properly handle FileList objects
        if (data.ResumeUrl instanceof FileList && data.ResumeUrl.length > 0) {
            formData.append("ResumeUrl", data.ResumeUrl[0])
        }
        
        // Log FormData contents
        console.log("FormData contents:")
        for (let pair of formData.entries()) {
            console.log(pair[0] + ': ', pair[1])
        }

        // Store the actual File objects, not just the FileList 
        const dataToStore = {
            ...data,
            ApplicantRegistrationId:applicantId,
            JobId:jobID,
            ResumeUrl: data.ResumeUrl instanceof FileList && data.ResumeUrl.length > 0 
            ? data.ResumeUrl[0] 
            : data.ResumeUrl,
        }


        const getFileFromData = (fileData) => {
            if (!fileData) return null;
            
            if (fileData instanceof File) {
            return fileData;
            }
            
            if (fileData instanceof FileList && fileData.length > 0) {
            return fileData[0];
            }
            
            if (typeof fileData === 'object' && fileData !== null) {
            if (fileData.name && fileData.type && fileData.size !== undefined) {
                try {
                const file = new File([], fileData.name, { 
                    type: fileData.type,
                    lastModified: fileData.lastModified || Date.now()
                });
                return file;
                } catch (error) {
                return null;
                }
            }
            }
            
        return null;
      };


    const pascalEmployee = toPascal(dataToStore);
      console.log(pascalEmployee)
        for (const key in pascalEmployee) {
            if (!pascalEmployee.hasOwnProperty(key)) continue;

            const value = pascalEmployee[key];

            if (value === undefined || value === null) continue;

            if (  key === 'ResumeUrl'  ) {
            const file = getFileFromData(value);
            if (file) {
                formData.append(key, file);
            }
        } else {
          formData.append(key, value.toString());
        }
      }

            // 🔍 Debug: print final FormData key/value pairs
console.log("🚀 FINAL FORMDATA BEFORE SUBMISSION -----------------");
for (const [key, value] of formData.entries()) {
  if (value instanceof File) {
    console.log(`${key}: [File] name=${value.name}, type=${value.type}, size=${value.size} bytes`);
  } else {
    console.log(`${key}: ${value}`);
  }
}
console.log("----------------------------------------------------");


      
        console.log("this my formdata",formData)
        const Apply = await hrmsAPI.createApply(formData)
        console.log("Applied: " , Apply)
        router.push('/NavBars/Home');
        } catch (error) {
        console.error('Register failed:', error);
        }
    };
  return (
    <div className='font-semibold space-y-17.75'>
        <div className='h-[14.82rem] w-full rounded-[1.875rem] bg-[url(/images/HomeBack.png)] pt-21 pl-20'>
            <div className='space-y-[17px]'>
                <h1 className='text-black font-bold text-4xl'>Blockchain Developer</h1>
                <h4 className='text-black'>Onyx Technology</h4>
            </div>
        </div>
        <div className='space-y-14.75'>
            <div className='flex items-center gap-1.75'>
                <img className='cursor-pointer' onClick={() => router.push('/NavBars/Home')} src="/Icons/ArrowLeft.png" alt="Back" />
                <h4 className='textWhite '>Block Chain</h4>
            </div>
            <div>

            </div>
            <form className='w-373 flex flex-col gap-13  ' onSubmit={handleSubmit(handleApply)}>
                <div className='w-full flex gap-13'>
                    <div className='w-180 flex flex-col gap-9.5'>     
                        <div className="flex flex-col gap-4 relative">
                        <label className="text-white">Name</label>
                        <div className="relative flex items-center w-full rounded-2.5 h-13.75">
                            <input
                            {...register('Name')}
                            className=" h-full w-full pl-3.75 placeholder:text-input rounded-[0.3125rem] border-2 border-[#1d2015] text-white focus:outline-none focus:border-lemongreen focus:ring-lemongreen bg-[#1D2015]"
                            placeholder='Enter Name'
                            />
                        </div>
                        {errors.Name && (
                            <span className="text-Error text-[1rem] absolute -bottom-8">{errors.Name.message}</span>
                        )}
                        </div>
                        <div className="flex flex-col gap-4 relative">
                            <label className="text-white">Phone Number</label>
                            <div className="relative flex items-center w-full rounded-2.5 h-13.75">
                                <input
                                className=" h-full w-full pl-3.75 placeholder:text-input rounded-[0.3125rem] border-2 border-[#1d2015] text-white focus:outline-none focus:border-lemongreen focus:ring-lemongreen bg-[#1D2015]"
                                placeholder='Enter Phone Number'
                                {...register('Phone')}
                                />
                            </div>
                            {errors.Phone && (
                            <span className="text-Error text-[1rem] absolute -bottom-8">{errors.Phone.message}</span>
                        )}
                        </div>
                    </div>
                    <div className='w-180 flex flex-col gap-9.5'>
                        <div className="flex flex-col gap-4 relative">
                            <label className="text-white">Email</label>
                            <div className="relative flex items-center w-full rounded-2.5 h-13.75">
                                <input
                                className=" h-full w-full pl-3.75 placeholder:text-input rounded-[0.3125rem] border-2 border-[#1d2015] text-white focus:outline-none focus:border-lemongreen focus:ring-lemongreen bg-[#1D2015]"
                                placeholder='Enter Email'
                                {...register('Email')}
                                />
                            </div>
                            {errors.Email && (
                            <span className="text-Error text-[1rem] absolute -bottom-8">{errors.Email.message}</span>
                        )}
                        </div>
                            {/* ResumeUrl */}
                            <div className='flex flex-col gap-4 relative '>
                                <label className='text-formColor'>Upload Cv</label>
                                <label className='inputModfile cursor-pointer border-none'>
                                    <img src='/Icons/File.png' alt='' />
                                    <span className='text-limeLight'>Upload Cv</span>
                                    <input type='file' className='hidden' 
                                    {...register('ResumeUrl')}
                                    />                               
                                    </label>
                                {errors.ResumeUrl && (
                            <span className="text-Error text-[1rem] absolute -bottom-8">{errors.ResumeUrl.message}</span>
                                )}
                            </div>
                    </div>
                </div>
                <div className='w-full h-13.75 bg-lemongreen  rounded-[10px]  '>
                    <button type="submit" className=' text-black w-full h-full text-center ' disabled={isSubmitting}  >{isSubmitting ? 'Submitting' : 'Submit'}</button>
                </div>         
            </form>
        </div>
    </div>
  )
}

export default page