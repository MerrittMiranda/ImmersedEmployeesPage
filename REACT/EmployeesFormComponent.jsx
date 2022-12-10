import React, { Fragment, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Col, Breadcrumb, Button, Form, Row, Tab } from "react-bootstrap";
import { ErrorMessage, Field, Formik, Form as FormikForm } from "formik";
import toastr from "toastr";
import debug from "sabio-debug";
import employeesService from "services/employeesService";
import * as formSchema from "../../schemas/employeeFormSchema";
import AddEmployeeForm from "./AddEmployeeForm";
import InviteMembersForm from "./InviteMembersForm";

const _logger = debug.extend("EmployeesFormComponent");

const EmployeesFormComponent = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [searchEmail, setEmailData] = useState({ email: "" });
  const [formData, setFormData] = useState({
    userData: {},
  });
  const [isVerified, setIsVerified] = useState(false);
  const [isNotVerified, setIsNotVerified] = useState(false);

  toastr.options = {
    toastClass: "analytics-toast-class",
    closeButton: false,
    debug: false,
    newestOnTop: true,
    progressBar: false,
    positionClass: "toast-top-right",
    preventDuplicates: true,
    onclick: null,
    showEasing: "swing",
    hideEasing: "linear",
    showMethod: "fadeIn",
    hideMethod: "fadeOut",
  };

  const onSearchClicked = (values) => {
    _logger(values, "should be email");
    const address = values.email;

    setEmailData(() => {
      return { email: address };
    });

    employeesService
      .searchEmail(address)
      .then(onSearchEmailSuccess)
      .catch(onSearchEmailError);
  };

  const onSearchEmailSuccess = (response) => {
    _logger(response, "Success: Email found.");
    toastr.success("Email exists.", "Verified");

    setIsVerified(true);

    let returnedUserData = response;

    setFormData((prevState) => {
      const pd = { ...prevState };
      pd.userData = returnedUserData.item;

      return pd;
    });
  };

  const onSearchEmailError = (error) => {
    _logger(error, "Error: This email is not in our database.");
    toastr.info("This email is not in our database", "What a Surprise!");

    setIsNotVerified(true);
    navigate(`/organization/${id}/employees/invitemember`);
  };

  const onGoToEmpClicked = () => {
    navigate(`/organization/${id}/employees`);
  };

  return (
    <Fragment>
      <Tab.Container defaultActiveKey="grid">
        <Row>
          <Col lg={12} md={12} sm={12}>
            <div className="border-bottom pb-4 mb-4 d-flex align-items-center justify-content-between">
              <div className="mb-3 mb-md-0">
                <h1 className="mb-1 h2 fw-bold">Add New Employee</h1>
                <Breadcrumb>
                  <Breadcrumb.Item>Dashboard</Breadcrumb.Item>
                  <Breadcrumb.Item onClick={onGoToEmpClicked}>
                    Employees
                  </Breadcrumb.Item>
                  <Breadcrumb.Item>Add</Breadcrumb.Item>
                </Breadcrumb>
                <div className="d-grid gap-2 d-md-block">
                  <Button
                    onClick={onGoToEmpClicked}
                    variant="primary"
                    className="me-1 ms-2 mt-2"
                    size="sm"
                  >
                    Go to Employees
                  </Button>
                </div>
              </div>
            </div>
          </Col>
        </Row>
      </Tab.Container>

      <Tab.Container defaultActiveKey="grid">
        <Tab.Content>
          <Tab.Pane eventKey="grid">
            <Col lg={12} md={12} sm={12}>
              <div className="card w-50 mx-auto">
                <div className="p-24 card-body">
                  <div className="mb-4">
                    {!isNotVerified ? (
                      <h1 className="mb-1 fw-bold border-bottom">
                        New Employee
                      </h1>
                    ) : (
                      <h1 className="mb-1 fw-bold border-bottom">
                        Invite Member
                      </h1>
                    )}
                  </div>
                  <Formik
                    enableReinitialize={true}
                    initialValues={searchEmail}
                    onSubmit={onSearchClicked}
                    validationSchema={formSchema.default.searchEmailSchema}
                  >
                    <FormikForm>
                      <Row className="align-items-center">
                        <Col sm={!isNotVerified ? 9 : 12} className="my-1">
                          <Form.Group as={Col}>
                            <Field
                              type="email"
                              name="email"
                              className="form-control"
                              placeholder="@ email address"
                            />
                            <ErrorMessage name="email" components="" />
                          </Form.Group>
                        </Col>

                        <Col xs="auto" className="my-1">
                          {!isNotVerified && (
                            <button
                              type="submit"
                              className={
                                !isVerified
                                  ? "me-1 btn btn-primary"
                                  : "me-1 btn btn-success disabled"
                              }
                            >
                              {!isVerified ? "Verify Email" : "Verified"}
                            </button>
                          )}
                        </Col>
                      </Row>
                    </FormikForm>
                  </Formik>
                  <Col>
                    {isVerified && (
                      <AddEmployeeForm newEmployee={formData.userData} />
                    )}
                    {isNotVerified && (
                      <InviteMembersForm inviteMember={searchEmail} />
                    )}
                  </Col>
                </div>
              </div>
            </Col>
          </Tab.Pane>
        </Tab.Content>
      </Tab.Container>
    </Fragment>
  );
};

export default EmployeesFormComponent;
