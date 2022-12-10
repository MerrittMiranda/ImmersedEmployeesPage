import * as Yup from "yup";

const signUpMemberSchema = Yup.object().shape({
  email: Yup.string().email("Invalid Email").required("Email is required"),
  firstName: Yup.string().min(1).max(125),
  lastName: Yup.string().min(1).max(125),
  avatarUrl: Yup.string().min(1).max(1000),
  phone: Yup.string().max(20),
  startDate: Yup.date(),
  password: Yup.string()
    .min(8)
    .required("Password is required")
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{8,})/,
      "Must Contain 8 Characters, One Uppercase, One Lowercase, One Number and One Special Case Character"
    ),
  passwordconfirm: Yup.string().oneOf(
    [Yup.ref("password"), null],
    "Must match password"
  ),
});

export default signUpMemberSchema;
