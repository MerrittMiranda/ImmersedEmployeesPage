import { lazy } from "react";


const InviteMembers = lazy(() =>
  import("../components/invitemembers/InviteMembers")
);

const routes = [
  {
    path: "/invitemembers/signup",
    name: "InviteMembers",
    exact: true,
    element: InviteMembers,
    roles: [],
    isAnonymous: true,
  },
];

const errorRoutes = [
  {
    path: "*",
    name: "Error - 404",
    element: PageNotFound,
    roles: [],
    exact: true,
    isAnonymous: true,
  },
];

var allRoutes = [
  ...routes,
  ...errorRoutes,
];

export default allRoutes;