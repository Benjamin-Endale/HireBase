'use client';
import React, {useState} from 'react';
import { useForm, Controller } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Dropdown } from '@/app/Components/DropDown';
import NumberInput from '@/app/Components/NumberInput';
import DropDownSearch from '@/app/Components/DropDownSearch';
// Categories
import { hrmsAPI } from '@/app/lib/api/client';
 
// ✅ Schema
const reviewSchema = z.object({
  employeeEmail: z.string().min(1, 'Employee is required'),
  reviewType: z.string().min(1, 'Review type is required'),
  ratings: z.array(z.number().min(0).max(5, 'Rating must be between 0 and 5')),
  feedbacks: z.array(z.string().min(5, 'Feedback must be at least 5 characters')),
  overallFeedback: z.string().min(10, 'Overall feedback is required'),
  
});

export default function AddReview({ onClose , users , questions , tenantId }) {
    const [selectedEmployee, setSelectedEmployee] = useState('');
    const [isSubmitting , setIsSubmitting] = useState(false);
  const {
    register,
    control,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
  } = useForm({
    resolver: zodResolver(reviewSchema),
    defaultValues: {
      reviewType: '',
      ratings: Array(questions.length).fill(0),
      feedbacks: Array(questions.length).fill(''),
      overallFeedback: '',
      employeeEmail: '',
    },
  });

  const ratings = watch('ratings');

  // Calculate average rating
  const averageRating =
    ratings.length > 0
      ? (ratings.reduce((sum, val) => sum + val, 0) / ratings.length).toFixed(1)
      : 0;


  if(questions.length === 0){

    return  <div className=' h-[53.9375rem] w-full flex center-center'>
          <div className='   flex flex-col items-center gap-4'>
                  <h4 className='font-semibold text-limegray w-full center-center'>No Questions Set!</h4>
                  <button onClick={onClose} className='text-lemongreen cursor-pointer'>Go Back</button>
                </div>
      </div>
  }

const onSubmit = async (data) => {
  try {
    setIsSubmitting(true);

    const payload = {
      employeeEmail: data.employeeEmail,
      tenantId: tenantId,                 
      reviewType: data.reviewType,
      reviewCycle: "2025",                 
      overallFeedback: data.overallFeedback,

      questions: questions.map((q, idx) => ({
        reviewQuestionId: q.id,             
        rating: data.ratings[idx],
        feedBack: data.feedbacks[idx],      
      })),
    };
 

    const result = await hrmsAPI.createPerformance(payload);
 
    onClose();
  } catch (err) {
    console.error("❌ Error saving Performance:", err.message || err);
  } finally {
    setIsSubmitting(false);
  }
};

  
  return (
    <div className="px-[3rem] py-[2.875rem] space-y-[3.125rem] font-semibold">
      {/* Header */}
      <div className="flex justify-between">
        <div>
          <h1 className="textFormColor">Performance Review</h1>
          <h4 className="text-limegray">
            Conduct a comprehensive performance evaluation
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
        className="flex flex-col gap-[4.75rem]"
      >
        {/* Dropdown */}
        <div className="flex justify-between w-full gap-[1.125rem]">
          <div className='w-[15.5625rem]'>
            <Controller
              name="employeeEmail"
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
                  displayName:    u.fullName
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
            {errors.employeeEmail && (
              <p className="text-Error text-[1rem]">
                {errors.employeeEmail.message}
              </p>
            )}
          </div>
          <div className="w-[15.5625rem]">
            <Controller
              name="reviewType"
              control={control}
              render={({ field }) => (
                <Dropdown
                  label="Review Type"
                  options={['Quarterly', 'Annual', 'Probation']}
                  selected={field.value}
                  onSelect={field.onChange}
                  placeholder="Select Review"
                />
              )}
            />
            {errors.reviewType && (
              <p className="text-Error text-[1rem]">{errors.reviewType.message}</p>
            )}
          </div>
        </div>

        {/* Category Ratings */}
        <div className="flex flex-col gap-[2.375rem]">
          {questions.length != 0 ? (questions.map((q, idx) => (
            <div key={idx} className="flex flex-col gap-[1rem]">
              <div className="between">
                <label className="text-formColor">{q.questionText}</label>
                <div className="flex gap-[0.8125rem] items-center">
                  <span className="text-limegray">
                    {ratings[idx]?.toFixed(1)}/5.0
                  </span>
                  <Controller
                    name={`ratings.${idx}`}
                    control={control}
                    render={({ field }) => (
                      <NumberInput
                        min={0}
                        max={5}
                        step={0.1}
                        defaultValue={field.value}
                        onChange={(val) => field.onChange(val)}
                      />
                    )}
                  />
                </div>
              </div>
              {errors.ratings?.[idx] && (
                <p className="text-Error text-[1rem]">
                  {errors.ratings[idx].message}
                </p>
              )}
              <textarea
                placeholder={`Feedback for ${q.questionText}`}
                className="text-formColor bg-inputBack rounded-[10px] placeholder-input pt-[0.59375rem] pl-[1.1875rem] resize-none h-[4.75rem]"
                {...register(`feedbacks.${idx}`)}
              />
              {errors.feedbacks?.[idx] && (
                <p className="text-Error text-[1rem]">
                  {errors.feedbacks[idx].message}
                </p>
              )}
            </div>
          ))) : (
            <div>
              <h4 className='text-lemongreen w-full center-center'>No Questions Set!</h4>
            </div>
          )}
        </div>

        {/* Overall Feedback */}
        <div className="flex flex-col gap-[1rem]">
          <div className="between">
            <label className="text-formColor">Overall Feedback</label>
          </div>
          <textarea
            placeholder="Feedback for overall performance"
            className="text-formColor bg-inputBack rounded-[10px] placeholder-input pt-[0.59375rem] pl-[1.1875rem] resize-none h-[4.75rem]"
            {...register('overallFeedback')}
          />
          {errors.overallFeedback && (
            <p className="text-Error text-[1rem]">
              {errors.overallFeedback.message}
            </p>
          )}
        </div>

        {/* Submit */}
        <div className="w-full h-[3.4375rem] mt-[0.5rem]">
          <button
            type="submit"
            className="w-full h-full bg-lemongreen rounded-[10px] cursor-pointer"
            disabled={isSubmitting ? true : false}
          >
            {isSubmitting ? 'Saving...' : 'Complete Review'}
          </button>
        </div>
      </form>
    </div>
  );
}
