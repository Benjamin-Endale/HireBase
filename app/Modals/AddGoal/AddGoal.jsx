'use client';
import React , {useState} from 'react';
import { useForm, Controller } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Dropdown } from '@/app/Components/DropDown';
import DropDownSearch from '@/app/Components/DropDownSearch';
import { hrmsAPI } from '@/app/lib/api/client';


const goalSchema = z.object({
  EmployeeEmail: z.string().min(1, 'Employee is required'),
  Priority: z.string().min(1, 'Priority is required'),
  Category: z.string().min(1, 'Category is required'),
  DueDate: z.string()
  .min(1, 'Due date is required')
  .refine(
      (value) => {
        if (!value) return false;
        const today = new Date();
        const dueDate = new Date(value);
        return dueDate >= today;
      },
      { message: 'Due Date must be today or later' }
    ),
  GoalTitle: z.string().min(3, 'Goal title must be at least 3 characters'),
  Description: z.string().min(10, 'Description is required'),
  
});

export default function AddGoal({ onClose , users , tenantId}) {
  const [selectedEmployee, setSelectedEmployee] = useState('');
  const [isSubmitting , setIsSubmitting] = useState(false);
  const {
    register,
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(goalSchema),
    defaultValues: {
      EmployeeEmail: '',
      Priority: '',
      Category: '',
      DueDate: '',
      GoalTitle: '',
      Description: '',
  
    },
  });

 

  const onSubmit = async (data) => {
  try {
     setIsSubmitting(true);
    const payload = {
      ...data,
      tenantId:tenantId,
    }
    const result = await hrmsAPI.createGoal(payload);
    console.log("This is the result: ",result)
    onClose();
  } catch (err) {
    console.error("❌ Error saving Performance:", err.message || err);
  } finally {
    setIsSubmitting(false);
  }
  };

  return (
    <div className="px-[3rem] py-[2.875rem] space-y-[3.125rem] font-semibold w-full">
      {/* Header */}
      <div className="flex justify-between">
        <div>
          <h1 className="textFormColor">Create New Goal</h1>
          <h4 className="text-limegray">
            Set up a new performance goal for an employee
          </h4>
        </div>
        <button
          onClick={onClose}
          className="rounded-full center-center cursor-pointer"
        >
          <img src="/image/Icon/Action/CloseCircle.png" alt="" />
        </button>
      </div>

      {/* Form */}
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="flex flex-col gap-[2.375rem]"
      >
        {/* Top section */}
        <div className="w-full flex gap-[1.125rem]">
          <div className="flex flex-col gap-[2.375rem] w-[15.5625rem]">
            {/* Employee */}
            <div>
            <Controller
              name="EmployeeEmail"
              control={control}
              render={({ field }) => (
              <DropDownSearch
                label="Employee"
                options={users.map((u) => ({
                  label: (
                    <div>
                      <div className="font-medium">{u.fullName}</div>
                      <div className="text-sm text-limegray">{u.email}</div>
                    </div>
                  ),
                  labelText: `${u.fullName} ${u.email}`, // for search
                  value:   u.email,
                  displayName:  u.fullName
                }))}
  
                selected={selectedEmployee}
                onSelect={(email, option) => {
                  setSelectedEmployee(option.displayName);  // UI shows name
                  field.onChange(email);                    // backend gets email
                }}

                placeholder="Search Employee..."
                 
              />

              )}
            />
            {errors.EmployeeEmail && (
              <p className="text-Error text-[1rem]">
                {errors.EmployeeEmail.message}
              </p>
            )}
            </div>

            {/* Priority */}
            <div>
              <Controller
                name="Priority"
                control={control}
                render={({ field }) => (
                  <Dropdown
                    label="Priority"
                    options={['High', 'Medium', 'Low']}
                    selected={field.value}
                    onSelect={field.onChange}
                    placeholder="Select priority"
                  />
                )}
              />
              {errors.Priority && (
                <p className="text-Error text-[1rem]">
                  {errors.Priority.message}
                </p>
              )}
            </div>
          </div>

          <div className="w-[15.5625rem] flex flex-col gap-[2.375rem]">
            {/* Category */}
            <div>
              <Controller
                name="Category"
                control={control}
                render={({ field }) => (
                  <Dropdown
                    label="Category"
                    options={['Behavioral', 'Emotional', 'Man']}
                    selected={field.value}
                    onSelect={field.onChange}
                    placeholder="Select category"
                  />
                )}
              />
              {errors.Category && (
                <p className="text-Error text-[1rem]">
                  {errors.Category.message}
                </p>
              )}
            </div>

            {/* Due Date */}
            <div className="flex flex-col w-full gap-[1rem]">
              <label className="textFormColor1">Due Date</label>
              <input
                type="date"
                className="inputMod pr-[1.5625rem]"
                {...register('DueDate')}
              />
              {errors.DueDate && (
                <p className="text-Error text-[1rem]">
                  {errors.DueDate.message}
                </p>
              )}
            </div>
          </div>
        </div>

        {/* Bottom section */}
        <div className="flex flex-col gap-[2.375rem]">
          {/* Goal Title */}
          <div className="flex flex-col gap-[1rem]">
            <label className="text-formColor">Goal Title</label>
            <input
              type="text"
              placeholder="e.g Improve Code Review Process"
              className="inputMod pr-[1.5625rem] placeholder-input"
              {...register('GoalTitle')}
            />
            {errors.GoalTitle && (
              <p className="text-Error text-[1rem]">{errors.GoalTitle.message}</p>
            )}
          </div>

          {/* Description */}
          <div className="flex flex-col gap-[1rem]">
            <label className="text-formColor">Description</label>
            <textarea
              placeholder="Detailed description of the goal and expected outcomes..."
              className="text-formColor bg-inputBack rounded-[10px] placeholder-input pt-[0.59375rem] pl-[1.1875rem] resize-none h-[5.5rem]"
              {...register('Description')}
            />
            {errors.Description && (
              <p className="text-Error text-[1rem]">
                {errors.Description.message}
              </p>
            )}
          </div>
        </div>

        {/* Submit Button */}
        <div className="w-full h-[3.4375rem] mt-[0.5rem]">
          <button
            type="submit"
            className={`w-full h-full ${isSubmitting ? 'bg-limegray' : ' bg-lemongreen'} rounded-[10px] cursor-pointer`}
            disabled={isSubmitting ? true : false}
          >
            {isSubmitting ? 'Creating...' : 'Create Goal'}
          </button>
        </div>
      </form>
    </div>
  );
}
