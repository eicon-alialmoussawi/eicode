import React, { useState, useCallback,useEffect } from "react";
import APIFunctions from "../utils/APIFunctions";
import { trackPromise } from "react-promise-tracker";
import BazSelector from "./BazSelector";
import FilterContainer from "./FilterContainer";
import ScrollableCheckboxList from "./CheckboxList";
import EIGenericTable from "./EIGenericTable";
import { Alert, LoadingAlert, AlertError } from "../components/f_Alerts";



// Table columns
const columnsDetails = [
  { Header: "Auction NO.", accessor: "auctionNumber" },
  { Header: "Year", accessor: "year" },
  { Header: "Operator", accessor: "operator" },
  { Header: "Term", accessor: "term_Y" },
  { Header: "Group", accessor: "group" },
  { Header: "Band", accessor: "band" },
  { Header: "Pair", accessor: "bandType" },
  { Header: "Block", accessor: "block_MHZ" },
  { Header: "Region", accessor: "region" },
  { Header: "County", accessor: "county" },
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
  var auctionCheckBoxAPI;
  var tableApi;
  const [isLoadingFilters, setIsLoadingFilters] = useState(true);
  const [auctions, setAuctions] = useState([]);
  const [awards, setAwards] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const handleSave = (selectedOption) => {
    console.log(`User selected: ${selectedOption}`);
    // Add custom logic (e.g., update state, make API call, etc.)
  };
  const loadAwards = function(payload)
  {
    return APIFunctions.getFilteredBazAwards(payload) 
  };
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
    auctionCheckBoxAPI = api;
  }, []);

  // Stabilized function for handling table API readiness
  const handleTableApiReady = useCallback((api) => {
    console.log("Table API is ready:", api);
    tableApi = api;
  }, []);

  // Handle search action
  const handleSearch = useCallback(() => {
    LoadingAlert("Show");
    var filter = {};
    filter.AuctionIds = auctionCheckBoxAPI.current.getSelectedValues();
    trackPromise(
      loadAwards(filter)
      .then(function(resp){
        console.log(resp.data)
        setAwards(resp.data);
        tableApi.load(resp.data);
        LoadingAlert("hide");
      })
      .catch((e) => {
        LoadingAlert("hide");
        console.log(e);
    })
    )
  }, []);
  useEffect(() => {
    APIFunctions.getAllBazAuctions()
        .then((resp) =>{ console.log(resp);
          setAuctions(resp.data);
        })
       
}, []);


  return (
    <div style={{ overflowY: 'auto', height: '100vh' }}>
        <FilterContainer onSearch={handleSearch}>
        {isLoadingFilters ? (
          <div style={{ textAlign: "center", padding: "20px" }}>
            <span>Loading filters...</span> {/* Replace with a spinner if desired */}
          </div>
        ) : (
          <div style={{ display: "flex", flexDirection: "row", gap: "1px" }}>
          <div
            data-title="Auctions"
            style={{
              cursor: "pointer",
              flex: "1", // Flex the div itself, not the content inside
              minWidth: "250px", 
              borderRight: "2px solid #ccc",// Prevents the div from shrinking too much
            }}
          >
            <ScrollableCheckboxList
              data={auctions}
              onReady={handleCheckboxApiReady}
              labelName="name"
              idName="id"
            />
          </div>
          <div
            data-title="Countries"
            onClick={() => setIsModalOpen(true)}
            style={{
              flex: "1", // Flex this div similarly
              minWidth: "150px", // Prevents shrinking too much
            }}
          >
            Hello
          </div>
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
      <BazSelector
  isOpen={isModalOpen}
  onClose={() => setIsModalOpen(false)}
  options={["Region", "States", "Counties"]} // Options for the user to select
  onSave={handleSave} // Function to handle the selected option
/>
    </div>
  );
};

export default ParentComponent;
