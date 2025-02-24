import React, { useState, useRef, useEffect } from "react";
import Modal from "react-bootstrap/Modal";
import ScrollableCheckboxList from "./CheckboxList";

const CheckboxModal = ({ isOpen, onClose, options, labelName, idName, onSave, title,defaultSelectedValues }) => {
  const checkboxApiRef = useRef(null); // Reference for the checkbox list API
  const [selectedValues, setSelectedValues] = useState([]); // Local state for selected values

  const handleSave = () => {
    if (checkboxApiRef.current) {
      const selected = checkboxApiRef.current.current.getSelectedValues(); // Get selected values from the API
      setSelectedValues(selected);
      onSave(selected); // Pass the selected values to the parent
    }
    onClose(); // Close the modal
  };

  useEffect(() => {
    if (checkboxApiRef.current) {
      checkboxApiRef.current.current.setSelectedValues(defaultSelectedValues); // Sync selected values when modal opens
    }
  }, [defaultSelectedValues]);

  return (
    <Modal show={isOpen} onHide={onClose} centered>
      <Modal.Header closeButton>
        <Modal.Title>{title}</Modal.Title> {/* Use dynamic title */}
      </Modal.Header>
      <Modal.Body>
        <ScrollableCheckboxList
          data={options}
          onReady={(api) => (checkboxApiRef.current = api)} // Get the ScrollableCheckboxList API
          labelName={labelName}
          idName={idName}
          height={160}
        />
      </Modal.Body>
      <Modal.Footer>
        <button className="btn btn-secondary" onClick={onClose}>
          Close
        </button>
        <button className="btn btn-primary" onClick={handleSave}>
          Save
        </button>
      </Modal.Footer>
    </Modal>
  );
};

export default CheckboxModal;
