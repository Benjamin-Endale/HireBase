import React from "react";
import clsx from "clsx";

const COLORS = [
  "bg-[#BEE532]",
  "bg-[#5D6150]",
  "bg-[#899072]",
  "bg-[#DFDFDF]",
];

const StackedLeaveProgress = ({ leaves, height }) => {
  // Sort leaves so they're grouped by type? Or keep original order?
  // Calculate total used vs total max
  const totalUsed = leaves.reduce((sum, leave) => sum + leave.used, 0);
  const totalMax = leaves.reduce((sum, leave) => sum + leave.max, 0);
  
  // We'll place bars sequentially based on used amount
  let currentPosition = 0;
  
  return (
    <div className="w-full overflow-visible">
      <div className="relative w-full rounded-[5px] h-9.5 p-[3px] bg-[rgba(190,229,50,0.05)]  border-hidden">
        {/* Empty background showing total available space */}
        <div className="absolute inset-0  top-0  h-full   rounded-[5px] bg-[url(/image/bar.png)] bg-no-repeat    "></div>
        
        {/* Stacked bars placed sequentially */}
        {leaves.map((leave, index) => {
          // Calculate width based on used amount (not percentage)
          const usedWidth = (leave.used / totalMax) * 100;
          
          const barStyle = {
            width: `${usedWidth}%`,
            left: `${currentPosition}%`
          };
          
          // Update position for next bar
          currentPosition += usedWidth;
          
          return (
            <div
              key={index}
              className="absolute top-0 group h-full rounded-[5px] space-x-[3px]"
              style={barStyle}
            >
              {/* Used bar - spans full width of its allocated used space */}
              <div
                className={clsx(
                  COLORS[index % COLORS.length],
                  height,
                  "transition-all duration-500 flex items-center justify-center rounded-[3px] "
                )}
              >
                {usedWidth >= 0 && (
                  <span className="text-black text-xs font-semibold whitespace-nowrap">
                    {Math.round((leave.used / leave.max) * 100)}%
                  </span>
                )}
              </div>

              {/* Tooltip */}
              <div
                className="
                  absolute -top-9 left-1/2 -translate-x-1/2
                  scale-0 group-hover:scale-100
                  transition-transform duration-200
                  bg-input text-limeLight text-xs
                  px-2 py-1 rounded-md whitespace-nowrap
                  z-50
                  after:content-['']
                  after:absolute after:left-1/2 after:-bottom-1
                  after:-translate-x-1/2
                  after:w-2 after:h-2
                  after:bg-black after:rotate-45
                "
              >
                {leave.label}: {leave.used}/{leave.max}
              </div>
            </div>
          );
        })}
      </div>
 
    </div>
  );
};

export default StackedLeaveProgress;


// import React from "react";
// import PropTypes from "prop-types";
// import clsx from "clsx";

// const COLOR_PALETTE = [
//   "bg-[#BEE532]",
//   "bg-[#5D6150]",
//   "bg-[#899072]",
//   "bg-[#DFDFDF]",
// ];

// const StackedLeaveProgress = ({ values, height }) => {
//   const valuesWithColors = values.map((item, index) => ({
//     ...item,
//     color: COLOR_PALETTE[index % COLOR_PALETTE.length],
//   }));

//   return (
//     <div className="w-full overflow-visible">
//       {/* progress bar */}
//       <div
//         className={clsx(
//           "w-full flex gap-[3px] rounded-[5px] bg-[rgba(190,229,50,0.05)]"
//         )}
//       >
//         {valuesWithColors.map((item, index) => (
//           <div
//             key={index}
//             className={clsx(
//               "relative group flex items-center justify-center rounded-[5px]",
//               item.color,
//               height
//             )}
//             style={{ width: `${item.percentage}%` }}
//           >
//             {/* Percentage */}
//             {item.percentage >= 10 && (
//               <span className="text-black text-xs font-semibold whitespace-nowrap z-10">
//                 {item.percentage}%
//               </span>
//             )}

//             {/* Tooltip */}
//             <div
//               className="
//                 absolute top-[-20] left-1/2 -translate-x-1/2
//                 scale-0 group-hover:scale-100
//                 transition-transform duration-200
//                 bg-input text-limeLight text-xs
//                 px-2 py-1 rounded-md whitespace-nowrap
//                 z-50
//                 after:content-['']
//                 after:absolute after:left-1/2 after:-bottom-1
//                 after:-translate-x-1/2
//                 after:w-2 after:h-2
//                 after:bg-black after:rotate-45
//               "
//             >
//               {item.label}
//             </div>
//           </div>
//         ))}
//       </div>
//     </div>
//   );
// };

// StackedLeaveProgress.propTypes = {
//   values: PropTypes.arrayOf(
//     PropTypes.shape({
//       label: PropTypes.string.isRequired,
//       percentage: PropTypes.number.isRequired,
//     })
//   ).isRequired,
//   height: PropTypes.string,
// };

// export default StackedLeaveProgress;
