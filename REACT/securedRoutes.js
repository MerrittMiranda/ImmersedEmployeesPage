import { lazy } from "react";
const AnalyticsDashboards = lazy(() =>
  import("../components/dashboard/analytics/Analytics")
);
const Employees = lazy(() => import("../components/employees/Employees"));
const EmployeesFormComponent = lazy(() =>
  import("../components/employees/EmployeesFormComponent")
);

const employeesRoute = [
  {
    path: "/organization/:id/employees",
    name: "Employees",
    element: Employees,
    roles: ["SysAdmin", "OrgAdmin"],
    exact: true,
    isAnonymous: false,
  },
  {
    path: "/organization/:id/employees/add",
    name: "EmployeesForm",
    element: EmployeesFormComponent,
    roles: ["SysAdmin", "OrgAdmin"],
    exact: true,
    isAnonymous: false,
  },
  {
    path: "/organization/:id/employees/invitemember",
    name: "EmployeesForm",
    element: EmployeesFormComponent,
    roles: ["SysAdmin", "OrgAdmin"],
    exact: true,
    isAnonymous: false,
  },
];

const errorRoutes = [
  {
    path: "*",
    name: "Error - 404",
    element: PageNotFound,
    roles: [],
    exact: true,
    isAnonymous: false,
  },
];

const allRoutes = [
  ...errorRoutes,
  ...employeesRoute,
];
export default allRoutes;