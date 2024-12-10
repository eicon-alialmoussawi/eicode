import React, { useState, useEffect, useMemo, useRef } from "react";
import { useTable, useSortBy, usePagination } from "react-table";
import { getValue } from "../Assets/Language/Entries";
import { getLang, getIMF, getPPP, displayPop } from "../utils/common";
import Modal from "react-bootstrap/Modal";
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';

const EIGenericTable = ({tabletitle,onReady,columnsDetails,itemToExportMapping}) => {
    const [isOpen, setIsOpen] = useState(false);
    const [title, setTitle] = React.useState("Transitioning...");
    const [isExportPDF, setisExportPDF] = useState(false);

    const [data, setData] = useState([]);
    const columns = useMemo(
        () => columnsDetails,
        [columnsDetails]
    );

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        page,
        nextPage,
        previousPage,
        canNextPage,
        canPreviousPage,
        pageOptions,
        gotoPage,
        pageCount,
        setPageSize,
        state,
        prepareRow,
    } = useTable(
        {
            columns,
            data: data,
            initialState: {
                hiddenColumns: columns.map((column) => {
                    if (column.show === false) return column.accessor || column.id;
                }),
            },
        },
        useSortBy,
        usePagination
    );
    const { pageIndex, pageSize } = state;
    const fileType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const fileExtension = '.xlsx';
    const exportToCSV = (csvData, fileName) => {


        var result = [];
        csvData.map((val, i) => {


           var item = itemToExportMapping(val);

            result.push(item);

        });

        const ws = XLSX.utils.json_to_sheet(result);
        const wb = { Sheets: { 'data': ws }, SheetNames: ['data'] };
        const excelBuffer = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
        const data = new Blob([excelBuffer], { type: fileType });
        FileSaver.saveAs(data, fileName + fileExtension);
    }
    const exportToPDF = () => {

      
        setisExportPDF(true);

        setTimeout(() => {
            setisExportPDF(false);
        }, 300);
    }
    const modalLoaded = () => {
        setTitle("Export");
    };

    const hideModal = () => {
        setIsOpen(false);
        setTitle("Export");
    };


    var api ={};
    api.load = function(data){
        setData(data);
    };
    React.useEffect(() => {
        if (typeof onReady === 'function') {
            onReady(api);
        }
    }, []);

    return ( 
        <div className="eigeneric-table">
            <div className="content_wrapper clearfix" style={{ paddingTop: 15, paddingBottom: 60 }}>
                    <div className="sections_group">
                        <div className="section_wrapper mcb-section-inner">
                            
                            <div className="wrap mcb-wrap one valign-top clearfix">
                                <div style={{ borderRadius: "10px" }} className="entry-content inner-entry-content px-4 py-3">
                                    <div className="row">
                                        <div className="col-12 text-left">
                                            <div className="row">
                                                <div className="col-12 text-left d-flex justify-content-between">
                                                    {/*Title*/}
                                                    <h4 className="text-blue-d text-bold" style={{ margin: 0 }}> {tabletitle}</h4>
                                                    {/*Actions*/}
                                                    <div className="tbl-actions">
                                                        <button disabled class="btn"><i class="fa fa-refresh"></i></button>
                                                        <button disabled class="btn"><i class="fa fa-chart-line"></i></button>
                                                        <button disabled={!data.length > 0} class={data.length > 0 ? "btn chart-btn-color" : "btn"} onClick={(e) => setIsOpen(true)}><i class="fa fa-download" ></i></button>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr style={{ margin: "5px 0" }} />
                                        </div>
                                    </div>

                                    {/*Table*/}
                                    <h6 className="fw-bold fs-14px" style={{ display: data.length === 0 ? "none" : "" }}>{getValue("NumberOfAwards", getLang())}: {data.length}</h6>
                                    <div
                                        id="table-content"
                                        className="col-md-12 list p-0 mb-3 custom-scrollbar"
                                    // style={{ height: numberOfAwards == 0 ? '' : '470px' }}
                                    >
                                        <table class="table table-width-auto fs-12px" role="table" id="table-to-xls">
                                            <thead>
                                                {headerGroups.map((headerGroup) => (
                                                    <tr {...headerGroup.getHeaderGroupProps()}>
                                                        {headerGroup.headers.map((column) => (
                                                            <th data-column={column.id} className={getLang() === "ar" ? "rtl" : "ltr"} {...column.getHeaderProps(column.getSortByToggleProps())}>
                                                                {/* <span>
                                  {column.isSorted ? column.isSortedDesc ? " ↓" : " ↑" : ""}{" "}
                                </span> */}
                                                                {column.render("Header")}
                                                            </th>
                                                        ))}
                                                    </tr>
                                                ))}
                                            </thead>
                                            <tbody {...getTableBodyProps()}>
                                                {page.map((row, i) => {
                                                    prepareRow(row);
                                                    return (
                                                        <tr className="socio-economic-tr strike-through"{...row.getRowProps()}>
                                                            {row.cells.map((cell) => {
                                                                return (
                                                                    <td data-column={cell.column.id} className={getLang() === "ar" ? "rtl" : "ltr"}  {...cell.getCellProps()}>
                                                                        {cell.render("Cell")}
                                                                    </td>
                                                                );
                                                            })}
                                                        </tr>
                                                    );
                                                })}
                                            </tbody>
                                        </table>
                                    </div>
                                    <div className="row" id="tableRow">
                                        <div className="col-md-6 d-flex align-items-start">
                                            <span className="text-black">
                                                {getValue("Page", getLang())}{" "}
                                                <strong>
                                                    {pageOptions.length !== 0 ? pageIndex + 1 : 0} {getValue("Of", getLang())} {pageOptions.length}
                                                </strong>{" "}
                                            </span>
                                            <span className="text-black d-flex ml-1 mr-1 align-items-start">
                                                | {getValue("GoToPage", getLang())}
                                                <input
                                                    type="number"
                                                    min="1"
                                                    max={pageOptions.length}
                                                    defaultValue={pageIndex + 1}
                                                    onChange={(e) => {
                                                        const pageNumber = e.target.value
                                                            ? Number(e.target.value) - 1
                                                            : 0;
                                                        gotoPage(pageNumber);
                                                    }}
                                                    style={{
                                                        width: "50px",
                                                        height: "25px",
                                                        borderBottom: 0,
                                                    }}
                                                    className="cstm-input ml-1"
                                                />
                                            </span>
                                        </div>
                                        <div className="col-6 d-flex justify-content-end">
                                            <button
                                                className="btn inner-btn-secondary px-5px py-0 me-1"
                                                onClick={() => gotoPage(0)}
                                                disabled={!canPreviousPage}>{"<<"}</button>
                                            <button className="btn inner-btn-secondary px-5px py-0 me-1"
                                                onClick={() => previousPage()}
                                                disabled={!canPreviousPage}>{getValue("Previous", getLang())}
                                            </button>
                                            <button className="btn inner-btn-secondary px-5px py-0 me-1"
                                                onClick={() => nextPage()}
                                                disabled={!canNextPage}>{getValue("Next", getLang())} </button>
                                            <button
                                                className="btn inner-btn-secondary px-5px py-0 me-1"
                                                onClick={() => gotoPage(pageCount - 1)}
                                                disabled={!canNextPage}>{">>"}</button>
                                        </div>
                                        </div>


                                    {/*End of Table */}
                                </div>
                        </div>
                    </div>
                </div>
            </div>
            <Modal
                show={isOpen}
                size="sm"
                onHide={hideModal}
                onEntered={modalLoaded}>
                <Modal.Header closeButton>
                    <Modal.Title>{title}</Modal.Title>
                    {/* <button type="button" className="btn-close" aria-label="Close"></button> */}
                </Modal.Header>
                <Modal.Body>
                    <ul class="list-group">
                        <li onClick={(e) => { exportToCSV(data, "Data") }} className="list-group-item" style={{ cursor: "pointer" }}> <span style={{ width: 110 }}><i
                            className="fas fa-file-excel"
                            style={{ paddingLeft: "10px", paddingRight: "10px", color: "#56ade0" }}></i>{getValue("ExportToExcel", getLang())}</span></li>
                        <li onClick={(e) => { exportToPDF() }} className="list-group-item" style={{ cursor: "pointer" }}> <span style={{ width: 110 }}><i className="fas fa-file-pdf"
                            style={{ paddingLeft: "10px", paddingRight: "10px", color: "#56ade0" }}></i>{getValue("ExportToPDF", getLang())}</span></li>
                        {/*isExportPDF && */}
                    </ul>
                </Modal.Body>
            </Modal>
        </div>
     );
}
 
export default EIGenericTable;