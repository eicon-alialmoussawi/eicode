import React, { useState, useCallback,useEffect } from "react";
import Modal from "react-bootstrap/Modal";

const styles = {
  boxStyle: (isSelected) => ({
    border: isSelected ? "2px solid #007bff" : "1px solid #ccc",
    padding: "16px",
    borderRadius: "8px",
    textAlign: "center",
    cursor: "pointer",
    marginBottom: "16px",
    backgroundColor: isSelected ? "#e3f2fd" : "white",
  }),
};

const BazSelector = ({ isOpen, onClose, options, onSave }) => {
  const [selected, setSelected] = useState("");

  const handleSelect = (option) => {
    setSelected(option);
    onSave(option); // Save the selected option immediately
    onClose(); // Close the modal
  };
console.log(options);
  return (
    <Modal show={isOpen} onHide={onClose} centered>
      <Modal.Header closeButton>
        <Modal.Title>Select an Option</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {options.map((option) => (
          <div
            key={option}
            style={styles.boxStyle(selected === option)}
            onClick={() => handleSelect(option)}
          >
            {option}
          </div>
        ))}
      </Modal.Body>
    </Modal>
  );
};

export default BazSelector;
