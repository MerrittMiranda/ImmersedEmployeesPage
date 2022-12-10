import React, { Fragment, useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Card, Col, Form, Row } from "react-bootstrap";
import { ErrorMessage, Field, Formik, Form as FormikForm } from "formik";
import toastr from "toastr";
import Swal from "sweetalert2";
import debug from "sabio-debug";
import inviteMemberService from "services/inviteMembersService";
import signUpMemberSchema from "schemas/signUpInviteMemberSchema";

const _logger = debug.extend("InviteMembers");

const InviteMembers = () => {
  const navigate = useNavigate();

  const [searchParams] = useSearchParams();
  const [formData, setFormData] = useState({
    user: {
      email: "",
      firstName: "",
      lastName: "",
      mi: "",
      avatarUrl: "",
      password: "",
      passwordConfirm: "",
    },
    employee: {
      phone: "",
      startDate: "",
      organizationId: "",
    },
  });

  useEffect(() => {
    const newState = {};

    searchParams.forEach((value, key) => {
      if (key === "email") {
        newState.email = value;
      }
      if (key === "firstName") {
        newState.firstName = value;
      }
      if (key === "lastName") {
        newState.lastName = value;
      }
      if (key === "organizationId") {
        newState.organizationId = Number(value);
      }
      _logger("key ,value", key, value);
    });
    setFormData(newState);
  }, [searchParams]);

  const onClickSubmit = (values) => {
    _logger("Values", values);

    const newFormData = {
      user: {
        email: values.email,
        firstName: values.firstName,
        lastName: values.lastName,
        mi: values.mi,
        avatarUrl: values.avatarUrl,
        password: values.password,
        passwordConfirm: values.passwordConfirm,
      },
      employee: {
        phone: values.phone,
        startDate: values.startDate,
        organizationId: values.organizationId,
      },
    };

    _logger(newFormData);
    inviteMemberService
      .signUpMember(newFormData)
      .then(onSignUpSuccess)
      .catch(onSignUpError);
  };

  const onSignUpSuccess = (response) => {
    _logger("Sign up was successful", response.item);
    Swal.fire({
      icon: "success",
      title: "Congratulations on joining Immersed!",
      text: "You will now be directed to the sign in page.",
      confirmButtonText: "Ok",
    }).then((result) => {
      if (result.isConfirmed) {
        navigate("/signin");
      }
    });
  };

  const onSignUpError = (error) => {
    _logger("Error: onSignUpError", error);
    toastr.error("Unable to complete Registration", "Error");
  };

  return (
    <Fragment>
      <Row className="align-items-center justify-content-center g-0 min-vh-100">
        <Col lg={4} md={4} className="py-8 py-xl-0">
          <Card>
            <Card.Body className="p-6">
              <div className="mb-4">
                <h1 className="mb-1 fw-bold">Invited to Join?</h1>
                <span>Enter your password below!</span>
              </div>
              <Formik
                enableReinitialize={true}
                initialValues={formData}
                onSubmit={onClickSubmit}
                validationSchema={signUpMemberSchema}
              >
                <FormikForm>
                  <Row>
                    <Col sm={12} className="my-1">
                      <Form.Group as={Col}>
                        <Field
                          type="email"
                          name="email"
                          className="form-control"
                          disabled
                        />
                        <ErrorMessage name="email" components="" />
                      </Form.Group>
                    </Col>

                    <Form.Group as={Col}>
                      <Form.Label className="pt-2">First Name</Form.Label>
                      <Field
                        type="text"
                        name="firstName"
                        className="form-control"
                      />
                      <ErrorMessage name="firstName" components="" />
                    </Form.Group>

                    <Form.Group as={Col}>
                      <Form.Label className="pt-2">Last Name</Form.Label>
                      <Field
                        type="text"
                        name="lastName"
                        className="form-control"
                      />
                      <ErrorMessage name="lastName" components="" />
                    </Form.Group>

                    <Form.Group as={Col}>
                      <Form.Label className="pt-2">
                        Middle Initial (Mi)
                      </Form.Label>
                      <Field
                        type="text"
                        name="mi"
                        className="form-control"
                        placeholder="Middle Initial"
                      />
                      <ErrorMessage name="mi" components="" />
                    </Form.Group>
                  </Row>
                  <Row>
                    <Form.Group as={Col}>
                      <Form.Label className="pt-2">Phone</Form.Label>
                      <Field
                        type="text"
                        name="phone"
                        className="form-control"
                        placeholder="***-***-****"
                      />
                      <ErrorMessage name="phone" components="" />
                    </Form.Group>

                    <Form.Group as={Col}>
                      <Form.Label className="pt-2">Start Date</Form.Label>
                      <Field
                        type="text"
                        name="startDate"
                        className="form-control"
                        placeholder="yyyy-mm-dd"
                      />
                      <ErrorMessage name="startDate" components="" />
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
                    <ErrorMessage name="avatarUrl" component="" />
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label className="pt-2">Password</Form.Label>
                    <Field
                      type="password"
                      name="password"
                      className="form-control"
                      placeholder="8 characters, 1 upper, 1 lower, 1 number, 1 symbol"
                    />
                    <ErrorMessage
                      name="password"
                      components=""
                      className="text-danger"
                    />
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label className="pt-2">Confirm Password</Form.Label>
                    <Field
                      type="password"
                      name="passwordConfirm"
                      className="form-control"
                      placeholder="Confirm Password"
                    />
                    <ErrorMessage
                      name="passwordConfirm"
                      components=""
                      className="text-danger"
                    />
                  </Form.Group>

                  <div className="d-grid gap-2 mx-auto col-md-6">
                    <button type="submit" className="me-1 btn btn-primary">
                      Register
                    </button>
                  </div>
                </FormikForm>
              </Formik>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Fragment>
  );
};

export default InviteMembers;
