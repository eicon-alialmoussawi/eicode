import React, { useState } from 'react';
import './Selector.css';

const Selector = ({ label, options, mode = 'multi', width = '100%', onChange }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selectedOptions, setSelectedOptions] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');

  // Toggle dropdown visibility
  const toggleDropdown = () => {
    setIsOpen(!isOpen);
  };
  
  // Handle option selection
  const handleOptionChange = (option) => {
    let updatedSelection;
    if (mode === 'multi') {
      // Toggle selection for multi mode
      console.log(selectedOptions);
      updatedSelection = selectedOptions.includes(option)
        ? selectedOptions.filter(item => item !== option)
        : [...selectedOptions, option];
    } else {
      // Single selection mode
      updatedSelection = [option]; // Only one option can be selected
    }
  
    setSelectedOptions(updatedSelection);
    console.log(updatedSelection);

    // Call the onChange prop with the updated selected options
    if (onChange) {
      onChange(updatedSelection);
    }
  };

  // Handle Select All
  const handleSelectAll = () => {
    const allOptions = options.map(opt => opt.value);
    setSelectedOptions(allOptions);

    // Call the onChange prop with the updated selected options
    if (onChange) {
      onChange(allOptions);
    }
  };

  // Handle Clear All
  const handleClearAll = () => {
    setSelectedOptions([]);

    // Call the onChange prop with an empty array
    if (onChange) {
      onChange([]);
    }
  };

  // Filter options based on search term
  const filteredOptions = options.filter(option =>
    option.label && option.label.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="dropdown" style={{ width }}>
      <div className="dropdown-label" onClick={toggleDropdown}>
        {label}
        <span className={`arrow ${isOpen ? 'open' : ''}`}>&#x25BC;</span>
      </div>
      {isOpen && (
        <div className="dropdown-content">
          <div className="search-container">
            <input
              type="text"
              className="search-input"
              placeholder="Search..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>

          {/* Scrollable Options List */}
          <div className="options-list">
            {filteredOptions.map(option => (
              <div className="option" key={option.value}>
                <div className="form-group">
                  <label className="chk-wrap">
                    <input
                      id={`option-${option.value}`}
                      type="checkbox"
                      checked={selectedOptions !== undefined && selectedOptions.includes(option.value)}
                      onChange={() => handleOptionChange(option.value)}
                      style={{ marginRight: '8px' }}
                    />
                    <span
                      onClick={(e) => {
                        e.preventDefault();
                        handleOptionChange(option.value);
                      }}
                      style={{ cursor: 'pointer' }}
                    >
                      {option.label}
                    </span>
                  </label>
                </div>
              </div>
            ))}
          </div>

          {/* Footer with Select All and Clear All */}
          <div className="dropdown-footer">
            <span className="select-all" onClick={handleSelectAll}>Select All</span>
            <span className="clear-all" onClick={handleClearAll}>Clear All</span>
          </div>
        </div>
      )}
    </div>
  );
};

export default Selector;
