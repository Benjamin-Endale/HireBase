import React, { useState } from "react";

const NumberInput = ({
  min = 0,
  max = 5,
  step = 1,
  defaultValue = 1,
  onChange,
}) => {
  const [value, setValue] = useState(defaultValue);

  const handleIncrease = () => {
    if (value < max) {
     const newValue = Math.max(min, parseFloat((value + step).toFixed(1)));
      setValue(newValue);
      onChange?.(newValue);
    }
  };

  const handleDecrease = () => {
    if (value > min) {
     const newValue = Math.min(max, parseFloat((value - step).toFixed(1)));
      setValue(newValue);
      onChange?.(newValue);
    }
  };

  return (
    <div className="flex flex-col gap-2">

      <button
        type="button"
        className=""
        onClick={handleIncrease}
        disabled={value >= max}
        aria-label="Increase value"
      >
        <img src="/image/Icon/vectorUP.png" alt="" />
      </button>

      <button
        type="button"
        className=""
        onClick={handleDecrease}
        disabled={value <= min}
        aria-label="Decrease value"
      >
        <img src="/image/Icon/vectorDown.png" alt="" />

      </button>

    </div>
  );
};

export default NumberInput;
