
namespace Anycmd.EDI.MessageServices {
	using Host.EDI;
	using Host.EDI.Entities;
	using Repositories;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;

	/// <summary>
	/// 状态码上下文
	/// </summary>
	public sealed class StateCodes : IEnumerable<StateCode> {
		private static readonly StateCodes _instance = new StateCodes();

		/// <summary>
		/// 
		/// </summary>
		public static StateCodes SingleInstance {
			get {
				return _instance;
			}
		}

		private static readonly List<StateCode> _stateCodes = new List<StateCode>();
		private static bool _initialized = false;
		private static object locker = new object();

		/// <summary>
		/// 构造并接入总线
		/// </summary>
		private StateCodes() {
			// TODO:总线接入
		}

		/// <summary>
		/// 
		/// </summary>
		internal static void Refresh() {
			if (_initialized) {
				_initialized = false;
			}
		}

		private void Init() {
			if (!_initialized) {
				lock (locker) {
					if (!_initialized) {
						var stateCodeEnumType = typeof(Status);
						var members = stateCodeEnumType.GetFields();
						var entities = new List<StateCode>();
						var ok = new StateCode();
						ok.Code = (int)Status.Ok;
						ok.ReasonPhrase = "Ok";
						ok.Description = "成功";
						entities.Add(ok);

						var stateCodeRepository = NodeHost.Instance.AppHost.GetRequiredService<IRepository<StateCode>>();
						var oldEntities = stateCodeRepository.FindAll().ToList();
						var newList = new List<StateCode>();
						var deleteList = new List<StateCode>();
						var updateList = new List<StateCode>();

						// 通过反射构建状态码对象
						foreach (var item in members) {
							if (item.DeclaringType == stateCodeEnumType) {
								var value = (Status)item.GetValue(Status.Ok);
								if (value != Status.Ok) {
									var description = string.Empty;
									object[] attrs = item.GetCustomAttributes(typeof(DescriptionAttribute), inherit:true);
									description = attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : item.Name;
									string reasonPhrase = value.ToString();
									if (oldEntities == null || oldEntities.Count == 0) {
										var entity = new StateCode();
										entity.Id = Guid.NewGuid();
										entity.Code = (int)value;
										entity.ReasonPhrase = reasonPhrase;
										entity.Description = description;
										entities.Add(entity);
										newList.Add(entity);
									}
									else if (oldEntities.All(c => c.ReasonPhrase != reasonPhrase)) {
										var entity = new StateCode();
										entity.Id = Guid.NewGuid();
										entity.Code = (int)value;
										entity.ReasonPhrase = reasonPhrase;
										entity.Description = description;
										entities.Add(entity);
										newList.Add(entity);
									} else {
										var entity = new StateCode();
										entity.Code = (int)value;
										entity.ReasonPhrase = reasonPhrase;
										entity.Description = description;
										entities.Add(entity);
										var old = oldEntities.FirstOrDefault(c => c.ReasonPhrase == reasonPhrase);
										if (old.Code != entity.Code) {
											old.Code = entity.Code;
											updateList.Add(old);
										}
									}
								}
							}
						}
						if (oldEntities != null) {
							foreach (var oldEntity in oldEntities) {
								var oldEntity1 = oldEntity;
								var entity = entities.FirstOrDefault(c => c.ReasonPhrase == oldEntity1.ReasonPhrase);
								// 删除废弃的
								if (entity == null) {
									deleteList.Add(oldEntity);
								}
								// 更新变化的
								else if (entity.Code != oldEntity.Code) {
									oldEntity.Code = entity.Code;
									updateList.Add(oldEntity);
								}
							}
						}

						if (deleteList.Count != 0) {
							foreach (var item in deleteList) {
								stateCodeRepository.Context.RegisterDeleted(item);
							}
						}
						if (updateList.Count != 0) {
							foreach (var item in updateList) {
								stateCodeRepository.Context.RegisterModified(item);
							}
						}
						if (newList.Count != 0) {
							foreach (var item in newList) {
								stateCodeRepository.Context.RegisterNew(item);
							}
						}

						if (deleteList.Count != 0 || updateList.Count != 0 || newList.Count != 0) {
							stateCodeRepository.Context.Commit();
						}
						_stateCodes.Clear();
						foreach (var item in entities.OrderBy(a => a.Code)) {
							_stateCodes.Add(item);
						}
						_initialized = true;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerator<StateCode> GetEnumerator() {
			if (!_initialized) {
				Init();
			}
			return _stateCodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			if (!_initialized) {
				Init();
			}
			return _stateCodes.GetEnumerator();
		}
	}
}
