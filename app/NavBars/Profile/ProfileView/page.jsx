'use client'
import React, { useState } from 'react'
import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { logout } from '@/app/lib/actions/auth';
import { hrmsAPI } from '@/app/lib/api/client';
import ModalContainerSignOut from '@/app/Modal/SignOut/ModalContainerSignOut';
import Signout from '@/app/Modal/SignOut/Signout';
const NameSchema = z.object({
  Name: z.string().min(2, "Name is required"),
});

const PasswordSchema = z
  .object({
    OldPassword: z
      .string()
      .min(8, "Password must be at least 8 characters long")
      .regex(/[A-Z]/, "Password must contain at least one uppercase letter")
      .regex(/[a-z]/, "Password must contain at least one lowercase letter"),
    NewPassword: z
      .string()
      .min(8, "New Password must be at least 8 characters long")
      .regex(/[A-Z]/, "New Password must contain at least one uppercase letter")
      .regex(/[a-z]/, "New Password must contain at least one lowercase letter"),
  })
  .superRefine((data, ctx) => {
    if (data.NewPassword === data.OldPassword) {
      ctx.addIssue({
        code: "custom",
        path: ["NewPassword"],
        message: "New password cannot be the same as the old password",
      });
    }
  });

const Page = ({ session , applicantId }) => {
  const [showPassword, setShowPassword] = useState(false);
  const [showOldPassword, setShowOldPassword] = useState(false);
  const [isSubmitting, setisSubmitting] = useState(false);
  const [isSubmitting1, setisSubmitting1] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [isOpen1, setIsOpen1] = useState(false);
    const {
      register: registerName,
      handleSubmit: handleSubmitName,
      formState: { errors: nameErrors },
    } = useForm({
      resolver: zodResolver(NameSchema),
      defaultValues: { Name: session?.user.fullname || "" },
    });

    const {
      register: registerPassword,
      handleSubmit: handleSubmitPassword,
      formState: { errors: passwordErrors },
    } = useForm({
      resolver: zodResolver(PasswordSchema),
      defaultValues: {
        OldPassword: "",
        NewPassword: "",
      },
    });

  // 🟢 Handle name change
  const handleName = async (data) => {
      try {
        setisSubmitting(true)
        const NameData = {
          FullName: data.Name,
          applicantId:applicantId
        };

        console.log("Name Data:  " , NameData)
        const changeName  = await  hrmsAPI.updateName(NameData)
         console.log("Name Data:  " , changeName)

         setIsOpen1(true)
      } catch (err) {
        console.error("❌ Error saving Name:", err.message || err);
      }  
  };

  // 🟢 Handle password change
  const handlePassword = async (data) => {
      try {
        setisSubmitting1(true)
        const PasswordData = {
          oldPassword: data.OldPassword,
          newPassword: data.NewPassword,
          applicantId:applicantId
        };

        console.log("Password Data:  " , PasswordData)
        const changePassword  = await  hrmsAPI.updatePassword(PasswordData)
         console.log("Password Data:  " , changePassword)
         setIsOpen(true)
      } catch (err) {
        console.error("❌ Error saving Name:", err.message || err);
      }  
  };

  return (
    <>
      <div className="font-semibold space-y-.5">

        {/* Profile Section */}
        <div className="h-72.25 center-center rounded-3xl bg-bgColor relative">
          <div className="flex flex-col justify-center items-center gap-6">
            <div className="rounded-full bg-lemongreen h-22.5 w-22.5 border-4 border-formColor" />
            <div className="flex flex-col items-center gap-[5px]">
              <h1 className="text-formColor text-2xl">{session?.user.fullname || 'No Name Found'}</h1>
              <h4 className="text-limegray font-medium">{session?.user.email || 'No Email Found'}</h4>
            </div>
          </div>
          <div className="absolute top-31.5 right-21.5 w-32.5 h-12.5">
            <button
              onClick={() => logout()}
              className="border cursor-pointer border-limegray bg-inherit py-10.125 px-3.875 w-full h-full rounded-[10px]"
            >
              <h4 className="text-formColor">Logout</h4>
            </button>
          </div>
        </div>

 

          <div className="h-122.25 w-full flex justify-center gap-20.5">
            {/* NAME SECTION */}
          <form onSubmit={handleSubmitName(handleName)}>
              <div className="flex flex-col gap-9.25 mt-18.5">
                <div className="flex flex-col gap-4">
                  <label className="text-white">Full Name</label>
                  <div className="relative flex items-center w-124.25 h-13.75">
                    <div className="absolute z-10 pl-4.75">
                      <img src="/Icons/userGray.png" alt="User icon" />
                    </div>
                    <input
                      {...registerName('Name')}
                      className="pl-[66px] h-full w-full placeholder:text-input rounded-[5px] border-2 border-[#1d2015] text-white focus:outline-none focus:border-lemongreen focus:ring-lemongreen bg-[#1D2015]"
                      type="text"
                      placeholder="ex. John Don"
                    />
                  </div>
                  {nameErrors.Name && <span className="text-Error">{nameErrors.Name.message}</span>}
                </div>

                <div className="flex flex-col gap-2 w-124.25">
                  <div className="bg-lemongreen w-full h-13.75 flex items-center justify-center rounded-[0.3125rem]">
                    <button
                      type="submit"
                      className="w-full h-full rounded-[0.3125rem] hover:outline-lemongreen cursor-pointer disabled:opacity-50 text-black font-semibold"
                      disabled = {isSubmitting ? true : false}
                    >
                      {isSubmitting ? 'Changing...' : 'Change Name'}
                    </button>
                    <ModalContainerSignOut open={isOpen1}>
                      <Signout Value="Name" onClose={()=>setIsOpen1(false)} />
                    </ModalContainerSignOut>
                  </div>
                </div>
              </div>
            </form>

            {/* Divider */}
            <div className="bg-bgColor w-[5px] h-full mt-15.5"></div>

            {/* PASSWORD SECTION */}
          <form onSubmit={handleSubmitPassword(handlePassword)}>
              <div className="flex flex-col gap-10.25 mt-18.5">
                {/* Old Password */}
                <div className="flex flex-col gap-4">
                  <label className="text-white">Old Password</label>
                  <div className="relative flex items-center w-124.25 h-13.75">
                    <div className="absolute   pl-4.75">
                      <img src="/Icons/LockPassword.png" alt="Password icon" />
                    </div>
                    <input
                      {...registerPassword('OldPassword')}
                      type={showOldPassword ? 'text' : 'password'}
                      className="pl-16.5 h-full w-full placeholder:text-input rounded-[0.3125rem] border-2 border-[#1d2015] text-white focus:outline-none focus:border-lemongreen focus:ring-lemongreen bg-[#1D2015]"
                      placeholder="*******************"
                    />
                    <button
                      type="button"
                      onClick={() => setShowOldPassword(!showOldPassword)}
                      className="absolute right-3 top-[50%] -translate-y-[50%]"
                    >
                      <img
                        src={showOldPassword ? '/Icons/HideEye.png' : '/Icons/eye.png'}
                        alt={showOldPassword ? 'Hide Password' : 'Show Password'}
                      />
                    </button>
                  </div>
                  {passwordErrors.OldPassword && <span className="text-Error">{passwordErrors.OldPassword.message}</span>}
                </div>

                {/* New Password */}
                <div className="flex flex-col gap-4">
                  <label className="text-white">New Password</label>
                  <div className="relative flex items-center w-124.25 h-13.75">
                    <div className="absolute   pl-4.75">
                      <img src="/Icons/LockPassword.png" alt="Password icon" />
                    </div>
                    <input
                      {...registerPassword('NewPassword')}
                      type={showPassword ? 'text' : 'password'}
                      className="pl-16.5 h-full w-full placeholder:text-input rounded-[0.3125rem] border-2 border-[#1d2015] text-white focus:outline-none focus:border-lemongreen focus:ring-lemongreen bg-[#1D2015]"
                      placeholder="*******************"
                    />
                    <button
                      type="button"
                      onClick={() => setShowPassword(!showPassword)}
                      className="absolute right-3 top-[50%] -translate-y-[50%]"
                    >
                      <img
                        src={showPassword ? '/Icons/HideEye.png' : '/Icons/eye.png'}
                        alt={showPassword ? 'Hide Password' : 'Show Password'}
                      />
                    </button>
                  </div>
                  {passwordErrors.NewPassword && <span className="text-Error">{passwordErrors.NewPassword.message}</span>}
                </div>

                {/* Update Password Button */}
                <div className="flex flex-col gap-2 w-124.25">
                  <div className="bg-lemongreen w-full h-13.75 flex items-center justify-center rounded-[0.3125rem]">
                    <button
                      type="button"
                      className="w-full h-full rounded-[0.3125rem] hover:outline-lemongreen cursor-pointer disabled:opacity-50 text-black font-semibold"
                      onClick={handleSubmitPassword(handlePassword)}
                      disabled = {isSubmitting1 ? true : false}

                    >
                      {isSubmitting1 ? 'Changing...' : 'Change'}
                    </button>
                    <ModalContainerSignOut open={isOpen}>
                      <Signout Value='Password' onClose={()=>setIsOpen(false)} />
                    </ModalContainerSignOut>
                  </div>
                </div>

              </div>
            </form>
          </div>

      </div>
    </>
  )
}

export default Page
