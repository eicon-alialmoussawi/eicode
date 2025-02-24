import React, { useState, useCallback, useEffect, useRef } from "react";
import APIFunctions from "../utils/APIFunctions";
import { trackPromise } from "react-promise-tracker";
import BazSelector from "./BazSelector";
import FilterContainer from "./FilterContainer";
import ScrollableCheckboxList from "./CheckboxList";
import CheckboxModal from "./CheckboxModal";
import EIGenericTable from "./EIGenericTable";
import { Alert, LoadingAlert, AlertError } from "../components/f_Alerts";

// Table columns
const columnsDetails = [
  { Header: "Auction NO.", accessor: "auctionNumber" },
  { Header: "Year", accessor: "year" },
  { Header: "Operator", accessor: "operator",className: "align-left" },
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
  // Refs for API components
  const auctionCheckBoxAPI = useRef(null);
  const regionCheckBoxAPI = useRef(null);
  const stateCheckBoxAPI = useRef(null);
  const countyCheckBoxAPI = useRef(null);
  const licenseCheckBoxAPI = useRef(null);
  const tableApi = useRef(null);

  const [isLoadingFilters, setIsLoadingFilters] = useState(true);
  const [auctions, setAuctions] = useState([]);
  const [licenses, setLicenses] = useState([]);
  const [awards, setAwards] = useState([]);
  const [regions, setRegions] = useState([]);
  const [counties, setCounties] = useState([]);
  const [states, setStates] = useState([]);
  const [locationBy, setLocationBy] = useState("Region");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isMarketModalOpen, setIsMarketModalOpen] = useState(false); // Modal state
  const [licenseSelectedValues, setLicenseSelectedValues] = useState([]);


  const openMarketModal = () => {
    setLicenseSelectedValues(licenseCheckBoxAPI.current?.current.getSelectedValues() || []);
    setIsMarketModalOpen(true);
  };
  const handleMarketModalSave = (selectedValues) => {
    setLicenseSelectedValues(selectedValues);
    if (licenseCheckBoxAPI.current) {
      licenseCheckBoxAPI.current.current.setSelectedValues(selectedValues); // Update outer CheckboxList
    }
  };

  const handleSave = (selectedOption) => {
    console.log(`User selected: ${selectedOption}`);
    setLocationBy(selectedOption);
  };

  const loadAwards = function (payload) {
    return APIFunctions.getFilteredBazAwards(payload);
  };

  // Handle search action
  const handleSearch = useCallback(() => {
    
    console.log("Current Auction API:", auctionCheckBoxAPI.current);

    const filter = {
        AuctionIds: auctionCheckBoxAPI.current?.current.getSelectedValues() || [],
        Licenses: licenseCheckBoxAPI.current?.current.getSelectedValues() || [],
    };

    // Add region/state/county selection based on locationBy
    if (locationBy === "Region") {
        filter.Regions = regionCheckBoxAPI.current?.current.getSelectedValues() || [];
    } else if (locationBy === "State") {
        filter.States = stateCheckBoxAPI.current?.current.getSelectedValues() || [];
    } else if (locationBy === "County") {
        filter.Counties = countyCheckBoxAPI.current?.current.getSelectedValues() || [];
    }

    // Check if any filter is an empty array
    const emptyFilters = Object.entries(filter).filter(([key, value]) => Array.isArray(value) && value.length === 0);

    if (emptyFilters.length > 0) {
        const emptyFilterNames = emptyFilters.map(([key]) => key).join(", ");
        Alert(`The following filters are empty: ${emptyFilterNames}`);
       
        console.log(`The following filters are empty: ${emptyFilterNames}`);
        return;
    }
    LoadingAlert("Show");

    // Proceed with API call if all filters are valid
    trackPromise(
        loadAwards(filter)
            .then((resp) => {
                console.log(resp.data);
                setAwards(resp.data);
                if (tableApi.current) {
                  tableApi.current.load(resp.data);
                }
                
                LoadingAlert("hide");
            })
            .catch((e) => {
                LoadingAlert("hide");
                console.error(e);
            })
    );
}, [locationBy]);


  useEffect(() => {
    setIsLoadingFilters(true); // Start loading
    Promise.all([
      APIFunctions.getAllBazAuctions(),
      APIFunctions.getAllBazRegions(),
      APIFunctions.getAllBazCounties(),
      APIFunctions.getAllBazStates(),
      APIFunctions.getAllBazLicense()
    ])
      .then(([auctionsResp, regionsResp, countiesResp, statesResp,licensesResp]) => {
        setAuctions(auctionsResp.data);
        setRegions(regionsResp.data);
        setCounties(countiesResp.data);
        setStates(statesResp.data);
        setLicenses(licensesResp.data);
      })
      .catch((error) => {
        console.error("Error loading filters:", error);
      })
      .finally(() => {
        setIsLoadingFilters(false);
      });
  }, []);

  return (
    <div style={{ overflowY: "auto", height: "100vh" }}>
      <FilterContainer onSearch={handleSearch}>
        {isLoadingFilters ? (
          <div style={{ textAlign: "center", padding: "20px" }}>
            <span>Loading filters...</span>
          </div>
        ) : (
          <div style={{ display: "flex", flexDirection: "row", gap: "1px" }}>
            <div
              data-title="Auction"
              style={{
                flex: "1",
                minWidth: "250px",
                borderRight: "2px solid #ccc",
              }}
            >
              <ScrollableCheckboxList
                data={auctions}
                onReady={(api) => (auctionCheckBoxAPI.current = api)}
                labelName="name"
                idName="id"
              />
            </div>
            <div
              data-title={locationBy}
              onClick={(e) => {
                if (e.target !== e.currentTarget) {
                  return;
                }
                setIsModalOpen(true);
              }}
              style={{
                flex: "1",
                cursor: "pointer",
                minWidth: "250px",
                borderRight: "2px solid #ccc",
              }}
            >
              {locationBy === "Region" && (
                <ScrollableCheckboxList
                  data={regions}
                  onReady={(api) => (regionCheckBoxAPI.current = api)}
                  labelName="name"
                  idName="id"
                />
              )}
              {locationBy === "State" && (
                <ScrollableCheckboxList
                  data={states}
                  onReady={(api) => (stateCheckBoxAPI.current = api)}
                  labelName="name"
                  idName="id"
                />
              )}
              {locationBy === "County" && (
                <ScrollableCheckboxList
                  data={counties}
                  onReady={(api) => (countyCheckBoxAPI.current = api)}
                  labelName="name"
                  idName="id"
                />
              )}
            </div>
            <div data-title="Market"   onClick={(e) => {
                if (e.target !== e.currentTarget) {
                  return;
                }
                openMarketModal();
              }}  style={{
                flex: "1",
                minWidth: "250px",
                borderRight: "2px solid #ccc",
              }}>
            <ScrollableCheckboxList
                  data={licenses}
                  onReady={(api) => (licenseCheckBoxAPI.current = api)}
                  labelName="name"
                  idName="id"
                />
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
              onReady={(api) => (tableApi.current = api)}
              columnsDetails={columnsDetails}
              itemToExportMapping={itemToExportMapping}
            />
          </div>
        </div>
      </div>
      <BazSelector
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        options={["Region", "State", "County"]} // Options for the user to select
        onSave={handleSave}
      />
       <CheckboxModal
        isOpen={isMarketModalOpen}
        onClose={() => setIsMarketModalOpen(false)}
        options={licenses}
        labelName="name"
        idName="id"
        onSave={handleMarketModalSave}
        title="Market"
        defaultSelectedValues={licenseSelectedValues}
      />
    </div>
  );
};

export default ParentComponent;
