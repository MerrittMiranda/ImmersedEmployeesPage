import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Col, Form, Row } from "react-bootstrap";
import { ErrorMessage, Field, Formik, Form as FormikForm } from "formik";
import PropTypes from "prop-types";
import toastr from "toastr";
import debug from "sabio-debug";
import employeesService from "services/employeesService";
import * as formSchema from "../../schemas/employeeFormSchema";

const _logger = debug.extend("EmployeesFormComponent");

const AddEmployeeForm = (props) => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [addEmployeeData, setFormField] = useState({
    email: "",
    userId: "",
    organizationId: id,
    firstName: "",
    lastName: "",
    avatarUrl: "",
    phone: "",
    startDate: "",
  });
  const newEmployee = props.newEmployee;
  _logger(newEmployee);

  useEffect(() => {
    setFormField((prevState) => {
      const pd = { ...prevState };
      pd.email = newEmployee.email;
      pd.userId = newEmployee.id;
      pd.firstName = newEmployee.firstName;
      pd.lastName = newEmployee.lastName;
      pd.avatarUrl = newEmployee.avatarUrl;
      return pd;
    });
  }, [props.newEmployee]);

  const onClickSubmit = (values) => {
    _logger("Values", values);

    employeesService
      .createEmployee(values)
      .then(onCreateSuccess)
      .catch(onCreateError);
  };

  const onCreateSuccess = (response) => {
    _logger("Employee added successfully", response.item);
    toastr.success("New Employee Added.", "Success");
    navigate(`/organization/${id}/employees`);
  };

  const onCreateError = (error) => {
    _logger("Error: onCreateError", error);
    toastr.error("Unable to Add Employee.", "Error");
  };

  return (
    <Formik
      enableReinitialize={true}
      initialValues={addEmployeeData}
      onSubmit={onClickSubmit}
      validationSchema={formSchema.default.addEmployeeSchema}
    >
      <FormikForm>
        <Row>
          <Form.Group as={Col}>
            <Form.Label className="pt-2">First Name</Form.Label>
            <Field
              type="text"
              name="firstName"
              className="form-control"
              placeholder="First Name"
            />
            <ErrorMessage name="firstName" components="" />
          </Form.Group>

          <Form.Group as={Col}>
            <Form.Label className="pt-2">Last Name</Form.Label>
            <Field
              type="text"
              name="lastName"
              className="form-control"
              placeholder="Last Name"
            />
            <ErrorMessage name="lastName" components="" />
          </Form.Group>
        </Row>

        <Form.Group className="mb-3">
          <Form.Label className="pt-2">Avatar Image</Form.Label>
          <Field
            type="text"
            name="avatarUrl"
            className="form-control"
            placeholder="Image url goes here"
          />
          <ErrorMessage name="avatarUrl" components="" />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Phone</Form.Label>
          <Field
            type="text"
            name="phone"
            className="form-control"
            placeholder="***-***-****"
          />
          <ErrorMessage name="phone" components="" />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Start Date</Form.Label>
          <Field
            type="text"
            name="startDate"
            className="form-control"
            placeholder="Select Date"
          />
          <ErrorMessage name="startDate" components="" />
        </Form.Group>

        <div className="d-grid gap-2 mx-auto col-md-6">
          <button type="submit" className="me-1 btn btn-primary">
            Submit
          </button>
        </div>
      </FormikForm>
    </Formik>
  );
};

AddEmployeeForm.propTypes = {
  newEmployee: PropTypes.shape({
    id: PropTypes.number.isRequired,
    avatarUrl: PropTypes.string.isRequired,
    email: PropTypes.string.isRequired,
    firstName: PropTypes.string.isRequired,
    lastName: PropTypes.string.isRequired,
    startDate: PropTypes.string,
    phone: PropTypes.string,
  }),
};

export default AddEmployeeForm;
