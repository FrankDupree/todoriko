import axios, { AxiosError } from 'axios';
import { toast } from 'react-toastify';

const API_URL = import.meta.env.VITE_API_URL;

export interface Todo {
    id: string;
    title: string;
    description?: string;
    isCompleted: boolean;
    createdAt: string;
    dueDate?: string;
    tag?: string;
    priority: PriorityLevel;
    position?: number;
}

export enum PriorityLevel {
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

export const fetchTodo = async (id: string) => {
    const response = await axios.get<Todo>(`${API_URL}/todos/${id}`);
    return response.data;
};

export const markComplete = async (id: string) => {
    try {
        const response = await axios.patch(`${API_URL}/todos/${id}/complete`);
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            const axiosError = error as AxiosError<{ errors?: Record<string, string[]> }>;

            if (axiosError.response?.data?.errors) {
                Object.values(axiosError.response.data.errors).flat().forEach(message => {
                    toast.error(message);
                });
            } else {
                toast.error('An error occurred');
            }
        } else {
            toast.error('An unexpected error occurred');
        }
        throw error; 
    }
}

export const fetchTodos = async (params?: {
    pageNumber?: number;
    pageSize?: number;
    titleFilter?: string;
    createdFrom?: string;
    createdTo?: string;
}) => {
    const response = await axios.get(`${API_URL}/todos`, { params });
    return response.data;
};

export const createTodo = async (todoData: Omit<Todo, 'id' | 'createdAt' | 'isCompleted'>) => {
    try {
        const payload = {
            ...todoData,
            priority: Number(todoData.priority),
            dueDate: todoData.dueDate?.length == 0 ? undefined : todoData.dueDate
        };
        const response = await axios.post(`${API_URL}/todos`, payload);
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            const axiosError = error as AxiosError<{ errors?: Record<string, string[]> }>;
            console.log(error)
            if (axiosError.response?.data?.errors) {
                Object.values(axiosError.response.data.errors).flat().forEach(message => {
                    toast.error(message);
                });
            } else {
                toast.error('An error occurred');
            }
        } else {
            toast.error('An unexpected error occurred');
        }
        throw error;
    }
};

export const updateTodo = async (id: string, todoData: Partial<Todo>) => {
    try {
        const payload = {
            ...todoData,
            priority: Number(todoData.priority),
            dueDate: todoData.dueDate?.length == 0 ? undefined : todoData.dueDate
        };
        const response = await axios.put(`${API_URL}/todos/${id}`, payload);
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            const axiosError = error as AxiosError<{ errors?: Record<string, string[]> }>;

            if (axiosError.response?.data?.errors) {
                Object.values(axiosError.response.data.errors).flat().forEach(message => {
                    toast.error(message);
                });
            } else {
                toast.error('An error occurred');
            }
        } else {
            toast.error('An unexpected error occurred');
        }
        throw error; 
    }
};

export const deleteTodo = async (id: string) => {
    await axios.delete(`${API_URL}/todos/${id}`);
};