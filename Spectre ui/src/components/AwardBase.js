import React, { useState, useCallback,useEffect } from "react";

import FilterContainer from "./FilterContainer";
import ScrollableCheckboxList from "./CheckboxList";
import EIGenericTable from "./EIGenericTable";

// Mock data for checkbox list
const data = [
  { operatorId: "us", name: "United States" },
  { operatorId: "ca", name: "Canada" },
  { operatorId: "mx", name: "Mexico" },
  { operatorId: "mx2", name: "Mexico2" },
  { operatorId: "mx3", name: "Mexico3" },
];

// Mock data for the table
const mockData = [
  {
    countryName: "United States",
    pop: 331000000,
    operatorName: "AT&T",
    year: 2024,
    upFrontFees: 20,
    terms: 10,
    band: "Band A",
    mhz: 10,
    pairing: "Yes",
    coverage: "90%",
  },
  {
    countryName: "Canada",
    pop: 38000000,
    operatorName: "Rogers",
    year: 2024,
    upFrontFees: 15,
    terms: 12,
    band: "Band B",
    mhz: 20,
    pairing: "No",
    coverage: "85%",
  },
];

// Table columns
const columnsDetails = [
  { Header: "Countries", accessor: "countryName" },
  { Header: "Pop", accessor: "pop" },
  { Header: "Operator", accessor: "operatorName" },
  { Header: "Date", accessor: "year" },
  { Header: "Price, $M", accessor: "upFrontFees" },
  { Header: "Term (Y)", accessor: "terms" },
  { Header: "Band", accessor: "band" },
  { Header: "Total MHz", accessor: "mhz" },
  { Header: "Pairing", accessor: "pairing" },
  { Header: "Coverage", accessor: "coverage" },
];

// Map item to export-friendly format
const itemToExportMapping = (val) => ({
  "Pop, M": val.pop ? (val.pop / 1_000_000).toFixed(3) : "",
  "Operator": val.operatorName,
  "Date": val.year,
  "Price, $M": val.upFrontFees,
  "Term (Y)": val.terms,
  "Band": val.band,
  "Total MHz": val.mhz,
  "Pairing": val.pairing,
  "Coverage": val.coverage,
});

const ParentComponent = () => {
  const [tableApi, setTableApi] = useState(null);
  const [isLoadingFilters, setIsLoadingFilters] = useState(true);
  useEffect(() => {
    // Simulate filter loading time
    const timer = setTimeout(() => {
      setIsLoadingFilters(false); // Set loading to false after some time
    }, 2000); // Adjust time as needed

    return () => clearTimeout(timer); // Cleanup timer
  }, []);

  // Stabilized function for handling checkbox API readiness
  const handleCheckboxApiReady = useCallback((api) => {
    console.log("Checkbox API is ready:", api);
  }, []);

  // Stabilized function for handling table API readiness
  const handleTableApiReady = useCallback(
    (api) => {
      if (tableApi !== api) {
        setTableApi(api);
        api.load(mockData); // Load data
      }
    },
    [tableApi]
  );

  // Handle search action
  const handleSearch = useCallback(() => {
    console.log("Search triggered");
  }, []);

  return (
    <div>
        <FilterContainer onSearch={handleSearch}>
        {isLoadingFilters ? (
          <div style={{ textAlign: "center", padding: "20px" }}>
            <span>Loading filters...</span> {/* Replace with a spinner if desired */}
          </div>
        ) : (
          <div data-title="Country Selection" style={{ cursor: "pointer" }}>
            <ScrollableCheckboxList
              data={data}
              onReady={handleCheckboxApiReady}
              labelName="name"
              idName="operatorId"
            />
          </div>
        )}
      </FilterContainer>

      <div id="Content" className="inner-content mt-3">
        <div
          className="content_wrapper clearfix"
          style={{ paddingTop: 15, paddingBottom: 60 }}
        >
          <div className="sections_group">
            <EIGenericTable
              tabletitle="Country Statistics Table"
              onReady={handleTableApiReady}
              columnsDetails={columnsDetails}
              itemToExportMapping={itemToExportMapping}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default ParentComponent;
