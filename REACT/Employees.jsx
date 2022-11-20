import React, { Fragment, useState } from "react";
import { useEffect } from "react";
import { Col, Row, Tab, Breadcrumb, Form, Button } from "react-bootstrap";
import { useNavigate, useParams } from "react-router-dom";
import "rc-pagination/assets/index.css";
import locale from "rc-pagination/lib/locale/en_US";
import Pagination from "rc-pagination";
import toastr from "toastr";
import debug from "sabio-debug";
import Employee from "./Employee";
import employeeService from "../../services/employeesService";

const _logger = debug.extend("Employees");

const Employees = () => {
  const { id } = useParams();
  const orgsId = id;

  const [employeesData, setEmployeesData] = useState({
    employees: [],
    employeeComponents: [],
    pageIndex: 0,
    pageSize: 8,
    totalCount: 0,
    initialIndex: 0,
  });
  const [searchTerm, setSearchTerm] = useState("");

  const navigate = useNavigate();

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

  useEffect(() => {
    searchTerm !== ""
      ? employeeService
          .search(
            employeesData.pageIndex,
            employeesData.pageSize,
            orgsId,
            searchTerm
          )
          .then(onSearchSuccess)
          .catch(onSearchError)
      : employeeService
          .getEmployeesPaginated(
            employeesData.pageIndex,
            employeesData.pageSize,
            orgsId
          )
          .then(onGetEmployeesSuccess)
          .catch(onGetEmployeesError);
  }, [employeesData.pageIndex]);

  const onGetEmployeesSuccess = (response) => {
    _logger(response);
    let employeesList = response.item.pagedItems;

    setEmployeesData((prevState) => {
      const employeeData = { ...prevState };
      employeeData.employees = employeesList;
      employeeData.employeeComponents = employeesList.map(mapEmployees);
      employeeData.pageIndex = response.item.pageIndex;
      employeeData.totalCount = response.item.totalCount;
      employeeData.pageSize = response.item.pageSize;
      return employeeData;
    });
  };

  const onGetEmployeesError = (error) => {
    _logger("Error: getEmployeesPaginated()", error);
    toastr.error("Error: unable to load employees", error);
  };

  const getSearchTerm = (event) => {
    let searchTerm = event.target.value;
    setSearchTerm(searchTerm);

    if (searchTerm !== "") {
      employeeService
        .search(
          employeesData.initialIndex,
          employeesData.pageSize,
          orgsId,
          searchTerm
        )
        .then(onSearchSuccess)
        .catch(onSearchError);
    } else {
      employeeService
        .getEmployeesPaginated(
          employeesData.pageIndex,
          employeesData.pageSize,
          orgsId
        )
        .then(onGetEmployeesSuccess)
        .catch(onGetEmployeesError);
    }
  };

  const onSearchSuccess = (response) => {
    _logger(response, "Search success");
    let employeesList = response.item;

    setEmployeesData((prevState) => {
      const employeeData = { ...prevState };
      employeeData.employees = employeesList.pagedItems;
      employeeData.totalCount = employeesList.totalCount;
      employeeData.employeeComponents =
        employeesList.pagedItems.map(mapEmployees);
      return employeeData;
    });
  };

  const onSearchError = (error) => {
    _logger("Error: onSearchError()", error);
    toastr.error("Error: search failed", error);
  };

  const mapEmployees = (singleEmployee) => {
    return (
      <Employee employee={singleEmployee} key={singleEmployee.employeeId} />
    );
  };

  const onPageChange = (page) => {
    setEmployeesData((prevState) => {
      let pageData = { ...prevState };
      pageData.pageIndex = page - 1;

      return pageData;
    });
  };

  const onAddEmployeeClicked = () => {
    navigate(`/organization/${id}/employees/add`);
  };

  return (
    <Fragment>
      <Tab.Container defaultActiveKey="grid">
        <Row>
          <Col lg={12} md={12} sm={12}>
            <div className="border-bottom pb-4 mb-4 d-flex align-items-center justify-content-between">
              <div className="mb-3 mb-md-0">
                <h1 className="mb-1 h2 fw-bold">Employees</h1>
                <Breadcrumb>
                  <Breadcrumb.Item>Dashboard</Breadcrumb.Item>
                  <Breadcrumb.Item>User</Breadcrumb.Item>
                  <Breadcrumb.Item>Employees</Breadcrumb.Item>
                </Breadcrumb>
              </div>
              <div className="d-grid gap-2 d-md-flex justify-content-md-end me-5">
                <Button
                  onClick={onAddEmployeeClicked}
                  variant="primary"
                  size="sm"
                >
                  Add New Employee
                </Button>
              </div>
            </div>
          </Col>
        </Row>

        <Tab.Content>
          <Tab.Pane eventKey="grid" className="pb-4">
            <div className="mb-4">
              <Form.Control
                type="search"
                placeholder="Search Employees"
                value={searchTerm}
                onChange={getSearchTerm}
              />
            </div>
            <Row>{employeesData.employeeComponents}</Row>
          </Tab.Pane>
        </Tab.Content>
      </Tab.Container>

      <Tab.Container defaultActiveKey="grid">
        <Row>
          <div className="border-bottom pb-4 mb-4 d-flex align-items-center justify-content-between">
            <Pagination
              showLessItems
              locale={locale}
              showTitle={false}
              current={employeesData.pageIndex + 1}
              onChange={onPageChange}
              pageSize={employeesData.pageSize}
              total={employeesData.totalCount}
              style={{ margin: "auto" }}
            />
          </div>
        </Row>
      </Tab.Container>
    </Fragment>
  );
};

export default Employees;
