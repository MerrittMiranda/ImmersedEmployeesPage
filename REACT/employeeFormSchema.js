import * as Yup from "yup";

const addEmployeeSchema = Yup.object().shape({
  email: Yup.string().email("Invalid Email").required("Email is required"),
  firstName: Yup.string().min(1).max(125),
  lastName: Yup.string().min(1).max(125),
  avatarUrl: Yup.string().min(1).max(1000),
  phoneNumber: Yup.string().max(20),
  startDate: Yup.date(),
});

const inviteMemberSchema = Yup.object().shape({
  email: Yup.string().email("Invalid Email").required("Email is required"),
  firstName: Yup.string().min(1).max(125),
  lastName: Yup.string().min(1).max(125),
});

const searchEmailSchema = Yup.object().shape({
  email: Yup.string().email("Invalid Email").required("Email is required"),
});

const employeeSchema = {
  addEmployeeSchema,
  inviteMemberSchema,
  searchEmailSchema,
};

export default employeeSchema;
