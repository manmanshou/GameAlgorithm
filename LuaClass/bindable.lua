---该函数创建一个可绑定的表（通常是model表)，后续可以让一个表bind上来
local __sync = __sync
local bindable = function (...)
    local cls = {}

    return setmetatable(cls, {
        __call = function()
            local data = {}
            for k, v in pairs(cls) do
                data[k] = v
            end

            local wrap = {
                __m_bind = {},
                __data = data,
               Count = function(self)
                   return #self.__data
               end,

               RemoveAt = function(self, index)
                   local l = #self.__data
                   if index > l then
                       error("index out of range")
                   end
                   self.__data[index] = nil
                   while index < l do
                       self.__data[index] = self.__data[index + 1]
                       index = index + 1
                   end
                   self.__data[l] = nil

                   __sync[self] = self
               end,

               Clear = function(self)
                   for i, v in ipairs(self.__data) do
                       self.__data[i] = nil
                   end

                   __sync[self] = self
               end,

               NotifyChanged = function(self)
                   UtilTool.DebugSync(UtilTool.DebugType.NotifyChanged,self,k,v)
                   __sync[self] = self
               end,

               Add = function(self, item)
                   table.insert(self.__data, item)

                   __sync[self] = self
               end,

               Get = function(self, index, t)
                   ---#if DEBUG
                   if(type(index) ~= "number") then
                       error("index 必须是整数")
                   end
                   ---#endif

                   local changed = false
                   while #self.__data < index do
                       table.insert(self.__data, t())
                       changed = true
                   end

                   if changed then
                       __sync[self] = self
                   end

                   return self.__data[index]
               end,

               Resize = function(self, sz)

                   ---#if DEBUG
                   if(type(sz) ~= "number") then
                       error("size 必须是整数")
                   end
                   ---#endif

                   while #self.__data > sz do
                       table.remove(self.__data)
                   end

                   --UtilTool.DebugSync(UtilTool.DebugType.Resize,self,k,v)
                   __sync[self] = self
               end,

               Length = function(self)
                  return #self.__data
               end,
                ---查找某个属性值相等的数组元素
               Find = function(self, propertyName, propertyValue)
                   for _,v in pairs(self.__data) do
                       if v[propertyName] == propertyValue then
                           return v
                       end
                   end
                   return nil
               end,
            }

            local mt = {
                --访问表里不存在的字段时
                --t:元表访问句柄 k:字段名 return data[k] 相当于对data的Get
                __index = function(t, k)
                    return data[k]
                end,

                --对表里不存在的字段赋值时触发
                --t:元表访问句柄 k:字段名 v:值 相当于对data的Set并同步
                __newindex = function(t, k, v)
                    local data = t.__data
                    data[k] = v

                    ---#if DEBUG
                    --local old = data[k]
                    --if t.__reassignable == nil or t.__reassignable[k] == nil then
                    --    if(type(old) == "table") then
                    --        local old_m_bind = rawget(old, "__m_bind")
                    --        if(old_m_bind ~= nil) and v ~= nil then
                    --            logger:Warning("不能对已经复制的bindable再次赋值，可以通过修改它的成员来改变数据。 The Key is " .. tostring(k))
                    --        end
                    --    end
                    --end
                    ---#endif

                    --local old_m_bind = rawget(old, "__m_bind")
                    UtilTool.DebugSync(UtilTool.DebugType.ValueChange,t,k,v)
                    __sync[t] = t
                end,
            }

            setmetatable(wrap, mt)
            return wrap
        end,

        __index = {
            MarkReassignable = function(self, k, b)
            end,

            ---#if DEBUG
            MarkReassignable = function(self, k, b)
                self.__reassignable = self.__reassignable or {}
                if b == true then
                    self.__reassignable[k] = true
                else
                    self.__reassignable[k] = nil
                end
            end,
            ---#endif
        }
    })
end

return bindable;
