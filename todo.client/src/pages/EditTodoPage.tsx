import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { fetchTodo, Todo, updateTodo } from '../api/todoApi';
import { TodoForm } from '../components/TodoForm';
import { ArrowLeftIcon } from '@heroicons/react/20/solid';

export function EditTodoPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const { data: todo, isLoading, isError } = useQuery({
        queryKey: ['todo', id],
        queryFn: () => fetchTodo(id!),
        enabled: !!id,
    });

    const updateMutation = useMutation({
        mutationFn: (data: Partial<Todo>) => updateTodo(id!, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['todos'] });
            queryClient.invalidateQueries({ queryKey: ['todo', id] });
            navigate('/todos');
        },
    });

    if (isLoading) return <div>Loading...</div>;
    if (isError) return <div>Error loading todo</div>;

    return (
        <div className="mx-auto max-w-4xl space-y-8 p-4 sm:p-6">
            <button
                onClick={() => navigate('/todos')}
                className="inline-flex items-center text-sm font-medium text-gray-700 hover:text-indigo-600"
            >
                <ArrowLeftIcon className="mr-1 h-5 w-5" />
                Back to Todos
            </button>

            <h1 className="text-2xl font-bold text-gray-900">Edit Todo</h1>

            {todo && (
                <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
                    <TodoForm
                        onSubmit={updateMutation.mutate}
                        defaultValues={todo}
                        isLoading={updateMutation.isPaused}
                        cancelCreating={() => navigate('/todos')}
                    />
                </div>
            )}
        </div>
    );
}