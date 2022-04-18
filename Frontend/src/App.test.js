import { render, screen, act } from '@testing-library/react'
import App from './App'
import ToDoItemsComponent from './components/TodoItemsComponent'
import AddTodoItemComponent from './components/AddTodoItemComponent'
import TodoItemApi from './api/TodoItem'

jest.mock('./api/TodoItem')

test('renders the footer text', () => {
  TodoItemApi.get = () => new Promise(() => { Promise.resolve()})
  render(<App />)
  const footerElement = screen.getByText(/clearpoint.digital/i)
  expect(footerElement).toBeInTheDocument()
})

test('renders todo items', () => {
  const items = [
    {
      id: 'id1',
      description: 'description 1'
    },
    {
      id: 'id2',
      description: 'description 2'
    }
  ]
  render(<ToDoItemsComponent items={items}></ToDoItemsComponent>)
  const elements = items.map((x) => screen.getByText(x.id))

  expect(elements[0].textContent).toEqual(items[0].id)
  expect(elements[1].textContent).toEqual(items[1].id)
})

test('calls item completed', () => {
  const mockPutItem = jest.fn(() => Promise.resolve())
  TodoItemApi.put = () => new Promise(mockPutItem)

  const items = [
    {
      id: 'id1',
      description: 'description 1'
    },
    {
      id: 'id2',
      description: 'description 2'
    }
  ]

  render(<ToDoItemsComponent items={items} onItemCompleted={mockPutItem}></ToDoItemsComponent>)

  const markAsCompletedButtons = screen.getAllByText('Mark as completed')
  markAsCompletedButtons[0].click()

  expect(mockPutItem).toBeCalled()
})

test('calls item added', () => {
  const mockPostItem = jest.fn(() => Promise.resolve())
  TodoItemApi.post = () => new Promise(mockPostItem)

  render(<AddTodoItemComponent onItemAdded={jest.fn()} onError={jest.fn()}></AddTodoItemComponent>)

  const formControl = screen.getByPlaceholderText('Enter description...')
  formControl.value = 'new todo'
  const addButton = screen.getAllByRole('button').find((x) => x.textContent == 'Add Item')
  addButton.click()

  expect(mockPostItem).toBeCalled()
})