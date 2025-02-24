import { getValue } from "../Assets/Language/Entries";
import { getLang } from "../utils/common";
import { useState } from "react";
import React from 'react';


const FilterContainer = (props) => {
  const {
    iconClassUp = "spectre-angle-up btn btn-primary background-color-2 color-white mr-2",
    iconClassDown = "spectre-angle-down btn btn-primary background-color-2 color-white mr-2",
    showTxt = "Show More",
    hideTxt = "Show Less",
    onSearch = () => {},
  } = props;

  const [showDisplay, setShowDisplay] = useState(false);

  const toggleDisplay = () => {
    setShowDisplay((prev) => !prev);
  };

  return (
    <div id="filters-box">
      {/* Access children via props */}
      {props.children ? (
        <div  class="wrap" style={{ display: showDisplay ? "" : "none" }}>
          {props.children}
        </div>
      ) : (
        <p>No filters available</p> // Fallback if no children are provided
      )}

      {/* Actions Section */}
      <div className="filter-actions" style={{ paddingTop: !showDisplay ? "20px" : undefined }}>
        <a id="toggle-filters" onClick={toggleDisplay}>
          <i className={showDisplay ? iconClassDown : iconClassUp}></i>{" "}
          {showDisplay ? hideTxt : showTxt}
        </a>
        <button
          type="submit"
          className="btn btn-primary background-color-2 mr-2"
          onClick={onSearch}
        >
          <i className="fa fa-search mr-1"></i> {getValue("Show", getLang())}
        </button>
      </div>
    </div>
  );
};

export default FilterContainer;
