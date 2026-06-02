import axios from "axios";

const API_URL = "http://localhost:5025/api";

export const getDashboardStats = async () => {
  const response = await axios.get(
    `${API_URL}/Admin/dashboard`
  );

  return response.data;
};

export const getBranches = async () => {
  const response = await axios.get(
    `${API_URL}/Branch`
  );

  return response.data;
};

export const getCounters = async () => {
  const response = await axios.get(
    `${API_URL}/Counter`
  );

  return response.data;
};

export const getUsers = async () => {
  const response = await axios.get(
    `${API_URL}/User`
  );

  return response.data;
};


export const createAgent = async (data: any) => {
  const response = await axios.post(
    `${API_URL}/User/create-agent`,
    data
  );

  return response.data;
};

export const getWaitingTickets = async () => {
  const response = await axios.get(
    `${API_URL}/Admin/waiting`
  );

  return response.data;
};

export const getServingTickets = async () => {
  const response = await axios.get(
    `${API_URL}/Admin/serving`
  );

  return response.data;
};

export const getCompletedTickets = async () => {
  const response = await axios.get(
    `${API_URL}/Admin/completed`
  );

  return response.data;
};

export const createBranch = async (data: any) => {
  const response = await axios.post(
    `${API_URL}/Branch`,
    data
  );

  return response.data;
};

export const updateBranch = async (
  id: string,
  data: any
) => {
  const response = await axios.put(
    `${API_URL}/Branch/${id}`,
    data
  );

  return response.data;
};

export const deleteBranch = async (
  id: string
) => {
  const response = await axios.delete(
    `${API_URL}/Branch/${id}`
  );

  return response.data;
};


export const createCounter = async (data: any) => {
  const response = await axios.post(
    `${API_URL}/Counter`,
    data
  );

  return response.data;
};

export const updateCounter = async (
  id: string,
  data: any
) => {
  const response = await axios.put(
    `${API_URL}/Counter/${id}`,
    data
  );

  return response.data;
};

export const deleteCounter = async (
  id: string
) => {
  const response = await axios.delete(
    `${API_URL}/Counter/${id}`
  );

  return response.data;
};