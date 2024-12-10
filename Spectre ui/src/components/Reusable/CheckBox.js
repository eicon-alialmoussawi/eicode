import React, { useState, useEffect } from "react";



// Checkbox component with an API object passed to the parent through onReady
const Checkbox = ({ label, initialValue, onChange, onReady }) => {
  const [checked, setChecked] = useState(initialValue);

  const handleChange = () => {
    const newValue = !checked;
    setChecked(newValue); // Toggle the state

    // Call the onChange prop if provided (for future use)
    if (onChange) {
      onChange(newValue); // Notify parent of state change (optional)
    }
  };

  // Create the API object to expose methods
  const api = {
    getValue: () => checked,
  };

  // When the component mounts, pass the api object to the parent via onReady
  useEffect(() => {
    if (onReady) {
      onReady(api); // Provide the API object to the parent
    }
  });

  return (
    <div className="form-group">
      <label className="chk-wrap">
        <input
          type="checkbox"
          checked={checked}
          onChange={handleChange} // Update internal state and notify parent
        />
        {label}
      </label>
    </div>
  );
};

export default Checkbox;
