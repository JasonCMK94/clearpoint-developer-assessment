const axios = require('axios')
const BASE_URL = "https://localhost:5001/api/todoitems"

const TodoItemApi =  {
  post: async function(data) {
    return axios({
      method: "post",
      url: BASE_URL,
      data: data
    })
  },
  put: async function(data) {
    return axios({
      method: "put",
      url: BASE_URL + "/" + data.id,
      data: data
    })
  },
  get: async function(data) {
    return axios({
      method: "get",
      url: BASE_URL
    })
  }
}

export default TodoItemApi