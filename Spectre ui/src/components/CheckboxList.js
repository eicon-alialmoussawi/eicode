import React, { useState, useEffect ,useRef} from "react";
import { getValue } from "../Assets/Language/Entries";
import { getLang } from "../utils/common";
var  updatedItems=[] ;
const ScrollableCheckboxList = ({
  data,
  onReady,
  labelName, // Name of the label field
  idName, // Name of the id field (e.g., 'operatorId', 'countryId', etc.)
}) => {
  const [checkAll, setCheckAll] = useState(false);
  const [checkedItems, setCheckedItems] = useState([]);
  const [searchTerm, setSearchTerm] = useState(""); // State for the search term
  const [filteredData, setFilteredData] = useState(data); // State for the filtered data

  // Update filtered data based on the search term
  useEffect(() => {
    const lowerCaseSearchTerm = searchTerm.toLowerCase();
    setFilteredData(
      data.filter((item) =>
        item[labelName]?.toLowerCase().includes(lowerCaseSearchTerm)
      )
    );
  }, [searchTerm, data, labelName]);

  // Handle the "Select All" checkbox change
  const handleCheckAll = (isChecked) => {
    setCheckAll(isChecked);
    if (isChecked) {
      updatedItems = filteredData.map((item) => item[idName]);
      setCheckedItems(updatedItems); // Select all filtered items
    } else {
      setCheckedItems([]); // Deselect all items
    }
  };

  // Handle individual checkbox change
  const handleCheck = (id, isChecked) => {
    setCheckedItems((prevState) => {
       updatedItems = isChecked
        ? [...prevState, id] // Add to checked list
        : prevState.filter((item) => item !== id); // Remove from checked list
      console.log(updatedItems);
      // Update checkAll if all filtered items are selected
      setCheckAll(updatedItems.length === filteredData.length);
      return updatedItems;
    });
  };

  // Update checkAll state when data changes (e.g., dynamically loaded)
  useEffect(() => {
    setCheckAll(
      checkedItems.length === filteredData.length && filteredData.length > 0
    );
  }, [checkedItems, filteredData]);

  // Render the list of checkboxes
  const renderOptions = () => {
    return filteredData.map((item) => (
      <div className="form-group" key={item[idName]}>
        <label className="chk-wrap">
          <input
            type="checkbox"
            checked={checkedItems.includes(item[idName])}
            onChange={(e) => handleCheck(item[idName], e.target.checked)}
          />
          {item[labelName]} {/* Use dynamic label passed in props */}
        </label>
      </div>
    ));
  };

  // Provide the API with the current selected items
  const apiRef = useRef({
    getSelectedValues: () => updatedItems, // Initially returns an empty array
  });

  useEffect(() => {
    onReady(apiRef); // Notify parent component about the API once component is ready
  }, []);

  return (
    <div className="checkbox-list">
      {/* Search Input */}
      <div className="form-group">
        <label className="lbl-icon-left">
          <span>
            <i className="spectre-search"></i>
          </span>
          <input
            type="text"
            onChange={(e) => setSearchTerm(e.target.value)} // Update search term on input
            placeholder={getValue("Search", getLang())} // Placeholder text for the search
          />
        </label>
      </div>
      <div className="form-group">
          <label className="chk-wrap">
            <input
              type="checkbox"
              onChange={(e) => handleCheckAll(e.target.checked)}
              checked={checkAll}
            />
            {getValue("SelectAll", getLang())}
          </label>
        </div>

      {/* Select All and Checkbox List */}
      <div className="scrollable bs-scrollable" style={{ height: 80 }}>
        {/* Select All Checkbox */}
   

        {/* Render the individual checkboxes */}
        <div>{renderOptions()}</div>
      </div>
    </div>
  );
};

export default ScrollableCheckboxList;
