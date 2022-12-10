import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Col, Form, Row } from "react-bootstrap";
import { ErrorMessage, Field, Formik, Form as FormikForm } from "formik";
import PropTypes from "prop-types";
import toastr from "toastr";
import Swal from "sweetalert2";
import debug from "sabio-debug";
import employeesService from "services/employeesService";
import * as formSchema from "../../schemas/employeeFormSchema";

const _logger = debug.extend("EmployeesFormComponent");

const InviteMembersForm = (props) => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [inviteMemberData, setInviteFormData] = useState({
    email: "",
    organizationId: id,
    firstName: "",
    lastName: "",
  });

  useEffect(() => {
    _logger(props);
    setInviteFormData((prevState) => {
      const pd = { ...prevState };
      pd.email = props.inviteMember.email;
      return pd;
    });
  }, [props.inviteMember]);

  const onSubmitInvite = (values) => {
    _logger("Values", values);

    employeesService
      .inviteMember(values)
      .then(onInviteSuccess)
      .catch(onInviteError);
  };

  const onInviteSuccess = (response) => {
    _logger("Invite member submitted successfully", response.item);

    Swal.fire({
      icon: "success",
      title: "Member invite sent!",
      text: "You have been directed back to the Employees page.",
      confirmButtonText: "Close",
    });

    navigate(`/organization/${id}/employees`);
  };

  const onInviteError = (error) => {
    _logger("Error: onInviteError", error);
    toastr.error("Could not submit Invite", "Error");
  };

  return (
    <Formik
      enableReinitialize={true}
      initialValues={inviteMemberData}
      onSubmit={onSubmitInvite}
      validationSchema={formSchema.default.inviteMemberSchema}
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

        <div className="d-grid gap-2 mx-auto col-md-6 pt-4">
          <button type="submit" className="me-1 btn btn-primary">
            Submit Invite
          </button>
        </div>
      </FormikForm>
    </Formik>
  );
};

InviteMembersForm.propTypes = {
  inviteMember: PropTypes.shape({
    email: PropTypes.string.isRequired,
  }),
};

export default InviteMembersForm;
