import axios from "axios";
import {
  onGlobalError,
  onGlobalSuccess,
  API_HOST_PREFIX,
} from "./serviceHelpers";

const endpoint = {
  employeeUrl: `${API_HOST_PREFIX}/api/employees`,
  userUrl: `${API_HOST_PREFIX}/api/users`,
};

const getEmployeesPaginated = (pageIndex, pageSize, orgsId) => {
  const config = {
    method: "GET",
    url: `${endpoint.employeeUrl}/organization/${orgsId}/?pageIndex=${pageIndex}&&pageSize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const search = (pageIndex, pageSize, orgsId, query) => {
  const config = {
    method: "GET",
    url: `${endpoint.employeeUrl}/organization/${orgsId}/search?pageIndex=${pageIndex}&&pageSize=${pageSize}&query=${query}`,
    withCredentials: true,
    crossDomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const searchEmail = (address) => {
  const config = {
    method: "GET",
    url: `${endpoint.userUrl}/email?address=${address}`,
    withCredentials: true,
    crossDomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const createEmployee = (payload) => {
  const config = {
    method: "POST",
    url: `${endpoint.employeeUrl}`,
    data: payload,
    withCredentials: true,
    crossDomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const inviteMember = (payload) => {
  const config = {
    method: "POST",
    url: `${endpoint.employeeUrl}/invitemembers`,
    data: payload,
    withCredntials: true,
    crossDomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const employeeService = {
  getEmployeesPaginated,
  search,
  searchEmail,
  createEmployee,
  inviteMember,
};

export default employeeService;
