import React from "react";
import { Col, Card, Image } from "react-bootstrap";
import PropTypes from "prop-types";
import { formatDate } from "utils/dateFormater";
import avatar from "../../assets/images/avatar/avatar.jpg";
import "./employees.css";

const Employee = (props) => {
  const employee = props.employee;

  return (
    <Col xl={3} lg={6} md={6} sm={12}>
      <Card className="mb-5">
        <Card.Body>
          <div className="text-center">
            <div className="position-relative">
              <Image
                className="employee-image"
                src={employee.avatarUrl ? employee.avatarUrl : avatar}
              />
            </div>
            <h4 className="mb-0">
              {employee.firstName} {employee.lastName}
            </h4>
          </div>
          <div className="d-flex justify-content-between border-bottom py-2">
            <span>Joined at </span>
            <span>{formatDate(employee.startDate)}</span>
          </div>
          <div className="d-flex justify-content-between border-bottom py-2">
            <span>Organization</span>
            <span>{employee.organization?.name}</span>
          </div>
          <div className="d-flex justify-content-between pt-2">
            <span>Email :</span>
            <span>{employee.email}</span>
          </div>
        </Card.Body>
      </Card>
    </Col>
  );
};

Employee.propTypes = {
  employee: PropTypes.shape({
    avatarUrl: PropTypes.string.isRequired,
    firstName: PropTypes.string.isRequired,
    lastName: PropTypes.string.isRequired,
    startDate: PropTypes.string.isRequired,
    email: PropTypes.string.isRequired,
    organization: PropTypes.shape({
      name: PropTypes.string.isRequired,
    }).isRequired,
  }),
};

export default Employee;
